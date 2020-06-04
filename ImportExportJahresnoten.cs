﻿using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;

namespace diNo
{

  /// <summary>
  /// Exportiert die Halbjahresnoten aus der elften Klasse ins nächste Jahr.
  /// </summary>
  public class ImportExportJahresnoten
  {
    /// <summary>
    /// Trennzeichen für csv-Export als char und string
    /// string benötigt, da sonst glatt Id 55+';'=114 ausgerechnet wird...
    /// </summary>
    private static char SepChar = ';';
    private static string Sep = "" + SepChar;
    // private static string FpAKennzeichen = "FpA-Teilnoten";
    // private static string FpAKuerzel = "FpA";
    private static Fach fpa; // enthält das Fach, unter dem die FPA ab der 12. Klasse als HjLeistung geführt wird

    /// <summary>
    /// Methode exportiert alle Noten in eine csv-Datei
    /// Format: ID;Name;Fachkürzel;HJPunkte;HJPunkte2Dez;HJSchnittMdl;HJArt
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ExportiereHjLeistungen(string fileName)
    {
      SucheFpa();

      using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      using (StreamWriter writer = new StreamWriter(stream))
      {
        foreach (Schueler schueler in Zugriff.Instance.SchuelerRep.getList())
        {
          Jahrgangsstufe jg = schueler.getKlasse.Jahrgangsstufe;
          foreach (var fachNoten in schueler.getNoten.alleFaecher)
          {           
            foreach (HjArt hjArt in new[] { HjArt.Hj1, HjArt.Hj2 })
            {
              HjLeistung note=null;
              if (jg ==Jahrgangsstufe.Elf)
                note = fachNoten.getHjLeistung(hjArt);
              else if (jg == Jahrgangsstufe.Zwoelf)
                note = fachNoten.getVorHjLeistung(hjArt); // für die Wiederholer

              SchreibeHj(writer, schueler, note);
            }
          }

          // Für 12/13 müssen nur Fremdsprachen exportiert werden
          // Sprachniveau immer, falls erreicht; Noten nur, sofern fürs Abizeugnis relevant 
          // (F-f in 12 oder 13 bzw. F in 13 für Wiederholer)
          if (jg > Jahrgangsstufe.Elf)
            foreach (var fachNoten in schueler.getNoten.alleSprachen)
            {
              Kursniveau kn = fachNoten.getFach.getKursniveau();
              HjArt[] hjArten;
              if (kn == Kursniveau.Englisch || kn==Kursniveau.Anfaenger && jg==Jahrgangsstufe.Zwoelf)
                hjArten = new[] { HjArt.Sprachenniveau };
              else 
                hjArten = new[] { HjArt.Hj1, HjArt.Hj2, HjArt.Sprachenniveau };

              foreach (HjArt hjArt in hjArten)
                SchreibeHj(writer, schueler, fachNoten.getHjLeistung(hjArt));
            }

          // und dann noch die fpA
          if (jg == Jahrgangsstufe.Elf && schueler.FPANoten != null && schueler.FPANoten.Count > 0)
          {
            foreach (var fpaZeile in schueler.FPANoten)
            {
              if (fpaZeile.IsGesamtNull())
                 continue;
                           
              writer.WriteLine(schueler.Id + Sep + fpa.Id + Sep + "11" + Sep + fpaZeile.Gesamt + Sep + fpaZeile.Halbjahr + Sep + schueler.NameVorname + Sep + fpa.Kuerzel);

              /* Die Fpa-Daten brauchen wir nicht mehr (im Notfall s. alter Notenbogen)
              // jahrespunkte sind null bei den fpaNoten aus dem ersten Halbjahr
              // vertiefung1 und vertiefung2 sind nur bei den Sozialen gefüllt
              // bemerkung und stelle sind für uns auch nicht so wichtig und dürften null sein
              byte? jahresPunkte = fpaZeile.IsJahrespunkteNull() ? (byte?)null : fpaZeile.Jahrespunkte;
              string bemerkung = fpaZeile.IsBemerkungNull() ? null : fpaZeile.Bemerkung.Replace("\n", ""); // mehrzeilig geht nicht mit csv
              byte? vertiefung1 = fpaZeile.IsVertiefung1Null() ? (byte?)null : fpaZeile.Vertiefung1;
              byte? vertiefung2 = fpaZeile.IsVertiefung2Null() ? (byte?)null : fpaZeile.Vertiefung2;
              string stelle = fpaZeile.IsStelleNull() ? null : fpaZeile.Stelle.Replace(";", ".").Replace("\n", "");
              
                // Erzeugt eine Zeile mit mind. 12 durch ; getrennten Werten (kann mehr sein, falls in der Bemerkung auch ;e enthalten sind)
                writer.WriteLine(schueler.Id + Sep + schueler.NameVorname + Sep + FpAKennzeichen + Sep + fpaZeile.Gesamt + Sep + fpaZeile.Halbjahr + Sep + jahresPunkte + Sep + fpaZeile.Vertiefung + Sep + vertiefung1 + Sep + vertiefung2 + Sep + fpaZeile.Anleitung + Sep + fpaZeile.Betrieb + Sep + stelle + Sep + bemerkung);
                // dasselbe als Hj-Leistung abspeichern: trage hierbei Gesamt auch als mdl. und Note2Dez ein (ist richtiger als leer lassen)
              */
            }
          }

        }
      }
    }

    public static void SchreibeHj(StreamWriter writer, Schueler schueler, HjLeistung note)
    {
      if (note != null && note.Status != HjStatus.Ungueltig)
      {
        // Erzeugt eine Zeile mit 9 durch ; getrennten Werten
        //                   0                       1                       2                       3                    4                       5                    
        writer.WriteLine(schueler.Id + Sep + note.getFach.Id + Sep + (int)note.JgStufe + Sep + note.Punkte + Sep + (byte)note.Art + Sep + schueler.NameVorname + " " + schueler.getKlasse.Bezeichnung + Sep + note.getFach.Kuerzel);
      }
    }

    private static void SucheFpa()
    {
      foreach (Fach f in Zugriff.Instance.FachRep.getList())
      {
        if (f.Typ == FachTyp.FPA)
        {
          fpa = f;
          break;
        }
      }
    }

    /// <summary>
    /// Importiert die Halbjahresleistungen der Schüler.
    /// Schreibt fehlerhafte Datensätze in ein ErrorFile.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    public static void ImportiereHJLeistungen(string fileName)
    {
      HjLeistungTableAdapter ada = new HjLeistungTableAdapter();
      //FachTableAdapter fta = new FachTableAdapter();
      //FpaTableAdapter fpata = new FpaTableAdapter();
      using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (StreamReader reader = new StreamReader(stream))
      using (StreamWriter writer = new StreamWriter(new FileStream(fileName+"_err.txt", FileMode.Create, FileAccess.ReadWrite)))
      {
        var schuelerAdapter = new SchuelerTableAdapter();
        while (!reader.EndOfStream)
        {
          string orignal = reader.ReadLine();
          string[] line = orignal.Split(SepChar);
          if (line.Length != 7) // Format sollte passen
          {
            writer.WriteLine(orignal);
            continue;
          }
          int schuelerId = int.Parse(line[0]);
          var schuelerGefunden = schuelerAdapter.GetDataById(schuelerId);
          if (schuelerGefunden == null || schuelerGefunden.Count == 0)
          {
            continue; // das ist normal. Schließlich sind auch die Halbjahresleistungen der letzten Absolventen noch mit in der Datei
          }

          Schueler schueler = new Schueler(schuelerGefunden[0]);
          Jahrgangsstufe jgSchueler = schueler.getKlasse.Jahrgangsstufe;
          if (jgSchueler <= Jahrgangsstufe.Elf)
            continue; // Schüler aus der 11. fangen vorne an (hier vermutlich Wiederholer), nix importieren
          
          int fachId = int.Parse(line[1]);
          Fach fach = Zugriff.Instance.FachRep.Find(fachId);

          Jahrgangsstufe jgstufeHj = (Jahrgangsstufe)int.Parse(line[2]);
          HjArt notenArt = (HjArt)byte.Parse(line[4]);

          // Importiere nur die Halbjahres-Noten der elften Klasse und immer die Sprachen
          if (jgstufeHj == Jahrgangsstufe.Elf && jgSchueler ==Jahrgangsstufe.Zwoelf || fach.getKursniveau()!=Kursniveau.None)
          {
            byte punkte = byte.Parse(line[3]);
            ada.Insert(schueler.Id, fachId, (byte)notenArt, punkte, null, null, (int)jgstufeHj, (byte)HjStatus.None);
          }
           

          /*
          if (line.Length >= 13)
          {
            // FpaZeile
            string nameVorname = line[1]; // nur zu Kontrollzwecken vorhanden
            string fach = line[2];
            if (fach != FpAKennzeichen)
            {
              throw new InvalidDataException("FpA-Zeile zu SchülerId " + schuelerId + " nicht korrekt. Erwartet: " + FpAKennzeichen + " enthielt: " + fach);
            }

            byte gesamt = byte.Parse(line[3]);
            byte halbjahr = byte.Parse(line[4]);
            byte? jahrespunkte = line[5] == "null" || line[5] == "" ? (byte?)null : byte.Parse(line[5]); // Jahrespunkte können null sein
            byte vertiefung = byte.Parse(line[6]);
            byte? vertiefung1 = line[7] == "null" || line[7] == "" ? (byte?)null : byte.Parse(line[7]);
            byte? vertiefung2 = line[8] == "null" || line[8] == "" ? (byte?)null : byte.Parse(line[8]);
            byte anleitung = byte.Parse(line[9]);
            byte betrieb = byte.Parse(line[10]);
            string stelle = line[11] == "null" ? null : line[11];

            string bemerkung = null;
            if (line[12] != "null")
            {
              List<string> bemerkungen = new List<string>();
              for (int i = 12; i < line.Length; i++) // im Normalfall nur eine Bemerkung. Aber falls jemand einen ; in der Bemerkung hatte...
              {
                bemerkungen.Add(line[i]);
              }

              bemerkung = string.Join(Sep, bemerkungen);
            }

            fpata.Insert(schuelerId, halbjahr, betrieb, anleitung, vertiefung, vertiefung1, vertiefung2, gesamt, stelle, bemerkung, jahrespunkte);
          }*/
        }
      }      
    }

    
    /// <summary>
    /// Sucht nach dem Fach in der Datenbank und liefert die Id zurück
    /// Wirft eine Exception wenn das Fach nicht gefunden werden kann
    /// </summary>
    /// <param name="fta">Fach Table Adapter.</param>
    /// <param name="fachKuerzel">Das eingelesene Fächerkürzel.</param>
    /// <returns>Die Id des Faches.</returns>
    private static int GetFachId(FachTableAdapter fta, string fachKuerzel)
    {
      var ergebnisFachSuche = fta.GetDataByKuerzel(fachKuerzel);
      if (ergebnisFachSuche == null || ergebnisFachSuche.Count == 0)
      {
        throw new InvalidDataException("ungültiges Fachkürzel in Halbjahresleistung " + fachKuerzel);
      }

      int fachId = ergebnisFachSuche[0].Id;
      return fachId;
    }
  }
}
