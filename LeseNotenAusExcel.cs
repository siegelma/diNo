﻿using System;
using System.Linq;
using System.Collections.Generic;
using diNo.diNoDataSetTableAdapters;
using System.Windows.Forms;
using System.IO;

namespace diNo
{
  /// <summary>
  /// Klasse zum Einlesen der Noten aus einer Excel-Datei und Eintragen der Noten in die Datenbank.
  /// </summary>
  public class LeseNotenAusExcel
  {
    private OpenNotendatei xls;
    private string fileName;
    private List<int> sidList = new List<int>(); // enthält die SIDs der Schüler, in der Reihenfolge wie im Excel
    private Kurs kurs; // der Kurs, den diese Datei abbildet
    public bool success = false;
    public StatusChanged StatusChanged;
    private List<string>hinweise = new List<String>();

    public LeseNotenAusExcel(string afileName, StatusChanged StatusChangedMethod, string sicherungsverzeichnis)
    {
      fileName = afileName;
      this.StatusChanged = StatusChangedMethod;

      // Datei sichern
      if (!string.IsNullOrEmpty(sicherungsverzeichnis))
      {
        try
        {
          File.Copy(fileName, sicherungsverzeichnis + Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.ToString("_yyMMdd_hhmmss") + Path.GetExtension(fileName));
        }
        catch
        {
          // wenn's nicht klappt, ist es halt so...
        }
      }
      xls = new OpenNotendatei(fileName);

      // Liste der gespeicherten Sids bereitstellen (alte Sids sollen nicht aus Excel gelöscht werden)
      for (int i = CellConstant.zeileSIdErsterSchueler; i < CellConstant.zeileSIdErsterSchueler + OpenNotendatei.MaxAnzahlSchueler; i++)
      {
        int sid = Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.SId + i));
        if (sid == 0) break; // wir sind wohl am Ende der Datei
        sidList.Add(sid);
      }

      kurs = Zugriff.Instance.KursRep.Find(Convert.ToInt32(xls.ReadValue(xls.sid, CellConstant.KursId)));

      Status("Synchronisiere Datei " + afileName);
      Synchronize();

      Status("Übertrage Noten aus Datei " + afileName);
      DeleteAlteNoten();
      UebertrageNoten();
      success = true;

      if (hinweise.Count > 0) // es sind Meldungen aufgetreten
        HinweiseAusgeben();

      // TODO: Gefährlich, private Variablen zu disposen?
      xls.Dispose();
      xls = null;

      Status("fertig mit Datei " + afileName);
    }

    private void Status(string meldung)
    {
      if (this.StatusChanged != null)
      {
        this.StatusChanged(this, new StatusChangedEventArgs() { Meldung = meldung });
      }
    }

    /// <summary>
    /// Gleicht die Schülerdaten zwischen DB und Excel ab. Prüft, ob neue Schüler hinzugekommen, oder ob Legasthenie neu vermerkt wurde.
    /// </summary>        
    private void Synchronize()
    {
      var klasse = kurs.getSchueler(true);
      
      foreach (var schueler in klasse)
      {
        // prüfen, ob neue Schüler dazugekommen sind
        if (!sidList.Contains(schueler.Id))
        {
          xls.AppendSchueler(schueler, kurs.getFach.Kuerzel == "F" || kurs.getFach.Kuerzel == "E");
          sidList.Add(schueler.Id);
          hinweise.Add(schueler.Name + ", " + schueler.Rufname + " wurde neu aufgenommen.");
        }
        // prüfen, ob Schüler reaktiviert wurden (SID steht zwar drin, aber Name ist gelöscht)
        else CheckAktiv(schueler);
        CheckLegastheniker(schueler);
      }

      // prüfen, ob Schüler entfernt werden müssen
      var klassenSIds = klasse.Select(x => x.Id);
      foreach (var schuelerId in sidList)
      {
        if (!klassenSIds.Contains(schuelerId))
        {          
          if (xls.RemoveSchueler(schuelerId))
          {
            Schueler schueler = Zugriff.Instance.SchuelerRep.Find(schuelerId);
            hinweise.Add(schueler.Name + ", " + schueler.Data.Rufname + " hat die Klasse verlassen.");
          }
        }
      }
    }

    private void CheckAktiv(diNoDataSet.SchuelerRow schueler)
    {
      // TODO: reaktivierte Schüler wieder mit Namen befüllen (kommt das oft vor?)
    }

    /// <summary>
    /// Prüft, ob die Legasthenievermerke der Datenbank mit der Excel-Datei übereinstimmen.
    /// </summary>
    /// <param name="schueler">Liste aller Schüler aus der Datenbank.</param>
    private void CheckLegastheniker(diNoDataSet.SchuelerRow schueler)
    {
      Schueler schuelerObj = new Schueler(schueler);
      bool isVermerkGesetzt = xls.GetLegasthenievermerk(schuelerObj.Id);
      bool sollteGesetztSein = schuelerObj.IsLegastheniker && (kurs.getFach.Kuerzel == "E" || kurs.getFach.Kuerzel == "F");
      if ((sollteGesetztSein && !isVermerkGesetzt) || (!sollteGesetztSein && isVermerkGesetzt))
      {
        string textbaustein = sollteGesetztSein ? "neu gesetzt" : "gelöscht";
        hinweise.Add("Bei " + schueler.Rufname + " " + schueler.Name + " wird der Legasthenievermerk "+ textbaustein + ". Sollte dies aus Ihrer Sicht nicht korrekt sein, wenden Sie sich bitte an das Sekretariat.");
        xls.SetLegasthenievermerk(schuelerObj.Id, sollteGesetztSein);
      }
    }

    /// <summary>
    /// Löscht die alten Noten dieses Kurses aus der Datenbank
    /// </summary>
    private void DeleteAlteNoten()
    {
      NoteTableAdapter ta = new NoteTableAdapter();
      ta.DeleteByKursId(kurs.Id);

      BerechneteNoteTableAdapter bta = new BerechneteNoteTableAdapter();
      bta.DeleteByKursId(kurs.Id);
    }


    /// <summary>
    /// Trägt die Noten eines Schülers aus Excel in die Datenbank ein.
    /// </summary>
    private void UebertrageNoten()
    {
      int i = CellConstant.ZeileErsterSchueler;
      int indexAP = CellConstant.APZeileErsterSchueler;

      foreach (int sid in sidList)
      {
        for (Halbjahr hj = Halbjahr.Erstes; hj <= Halbjahr.Zweites; hj++)
        {
          foreach (Notentyp typ in Enum.GetValues(typeof(Notentyp)))
          {
            if (typ==Notentyp.Kurzarbeit) continue; // wird unter Ex bearbeitet

            string[] zellen = CellConstant.getLNWZelle(typ, hj, i, indexAP);
            foreach (string zelle in zellen)
            {
              byte? p = xls.ReadNote(typ, zelle);
              if (p != null)
              {
                Note note = new Note(kurs.Id, sid);
                note.Halbjahr = hj;
                // Gewichtung steht bei Ex auf 2, also KA
                if ((typ==Notentyp.Ex) && (xls.ReadValue(xls.notenbogen,Char.ToString(zelle[0]) + CellConstant.GewichteExen)=="2"))
                   { note.Typ = Notentyp.Kurzarbeit; }
                else
                   { note.Typ = typ; }

                note.Zelle = zelle;
                note.Punktwert = (byte)p;
                note.writeToDB();
              }
            }
          }

          BerechneteNote bnote = new BerechneteNote(kurs.Id, sid);
          bnote.ErstesHalbjahr = (hj == Halbjahr.Erstes);
          bnote.SchnittSchulaufgaben = xls.ReadSchnitt(BerechneteNotentyp.SchnittSA, hj, i);
          bnote.SchnittMuendlich = xls.ReadSchnitt(BerechneteNotentyp.Schnittmuendlich, hj, i);
          bnote.JahresfortgangMitKomma = xls.ReadSchnitt(BerechneteNotentyp.JahresfortgangMitNKS, hj, i);
          bnote.JahresfortgangGanzzahlig = xls.ReadSchnittGanzzahlig(BerechneteNotentyp.Jahresfortgang, hj, i);
          bnote.PruefungGesamt = xls.ReadSchnitt(BerechneteNotentyp.APGesamt, hj, indexAP);
          bnote.SchnittFortgangUndPruefung = xls.ReadSchnitt(BerechneteNotentyp.EndnoteMitNKS, hj, indexAP);
          bnote.Abschlusszeugnis = xls.ReadSchnittGanzzahlig(BerechneteNotentyp.Abschlusszeugnis, hj, indexAP);

          // für die PZ im 1. Hj. reicht ggf. auch eine mündliche Note
          // im 2. Hj. kann das nicht so einfach übernommen werden, da Teilnoten aus dem 1. Hj. feststehen
          if (bnote.JahresfortgangGanzzahlig == null && bnote.ErstesHalbjahr)
          {
               if (bnote.SchnittMuendlich !=null) bnote.JahresfortgangMitKomma = bnote.SchnittMuendlich;
               else if (bnote.SchnittSchulaufgaben !=null) bnote.JahresfortgangMitKomma = bnote.SchnittSchulaufgaben;
               bnote.RundeJFNote();
          }

          // Nur wenn einer der Schnitte feststeht, wird diese Schnittkonstellation gespeichert
          if (bnote.SchnittMuendlich != null || bnote. SchnittSchulaufgaben != null)
            bnote.writeToDB();
        }
         
        i += 2;
        indexAP++;
      }
    }

    private void HinweiseAusgeben()
    {
      string s="";
      foreach (var h in hinweise)
      {
        s += h + "\n";
      }
      if (MessageBox.Show(s + "\nSollen obige Änderungen in Ihre Notendatei übernommen werden.", Path.GetFileNameWithoutExtension(fileName), MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes) 
        xls.workbook.Save();
    }
  }
}
