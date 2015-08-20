﻿using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class Notenbogen : Form
  {
    private Schueler schueler;
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="schuelerId">Die Id des anzuzeigenden Schülers.</param>
    public Notenbogen(int schuelerId)
    {
        log.Debug("Öffne Notenbogen SchülerId=" + schuelerId);
        InitializeComponent();

        schueler = new Schueler(schuelerId);
        nameTextBox.Text = schueler.NameVorname;
        klasseTextBox.Text = schueler.getKlasse.Data.Bezeichnung;
        textBoxAdresse.Text = schueler.Data.AnschriftStrasse + "\n" + schueler.Data.AnschriftPLZ + " " + schueler.Data.AnschriftOrt + "\n Tel.:" + schueler.Data.AnschriftTelefonnummer;

        SchuelerNoten noten = schueler.getNoten;

        // Row[lineCount] für schriftliche und Row[lineCount+1] für mündliche Noten
        int lineCount = 0;
        int col;
        foreach(var kursNoten in noten.alleFaecher)
        {
            dataGridNoten.Rows.Add(2);
            dataGridNoten.Rows[lineCount + 1].Height += 2;
            dataGridNoten.Rows[lineCount + 1].DividerHeight = 2;
            dataGridNoten.Rows[lineCount].Cells[0].Value = kursNoten.getFach.Bezeichnung;

            InsertNoten(1, lineCount, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Schulaufgabe));
            col = InsertNoten(1, lineCount+1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Ex));
            col = InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Ex));
            col = InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.EchteMuendliche));
            col = InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Fachreferat), "F");
                  InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Ersatzprüfung), "E");

            InsertNoten(11, lineCount, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Schulaufgabe));
            col = InsertNoten(11, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Kurzarbeit),"K");
            col = InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Ex));
            col = InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.EchteMuendliche));
            col = InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Fachreferat), "F");
                  InsertNoten(col, lineCount + 1, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Ersatzprüfung), "E");

            InsertSchnitt(9,lineCount,kursNoten.getSchnitt(Halbjahr.Erstes));
            InsertSchnitt(20,lineCount, kursNoten.getSchnitt(Halbjahr.Zweites));

/*
            InsertSingleNote(22, lineCount, noten, Notentyp.APSchriftlich, Halbjahr.Ohne, false);
            InsertSingleNote(23, lineCount, noten, Notentyp.APMuendlich, Halbjahr.Ohne, false);

            dataGridNoten.Rows[lineCount].Cells[24].Value = berechneteZweitesHJ.PruefungGesamt.ToString();
            dataGridNoten.Rows[lineCount].Cells[25].Value = berechneteZweitesHJ.Abschlusszeugnis.ToString();
*/
            lineCount = lineCount + 2;
        }

        if (schueler.Data.Jahrgangsstufe == "11")
        {
          FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
          var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
          if (fpANoten.Count == 1)
          {
            textBoxFpABemerkung.Text = fpANoten[0].Bemerkung;
            listBoxFpA.SelectedIndex = fpANoten[0].Note;
          }
        }

        if (schueler.Data.Jahrgangsstufe == "13")
        {
          SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
          var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
          if (seminarfachnoten.Count == 1)
          {
            numericUpDownSeminarfach.Value = seminarfachnoten[0].Gesamtnote;
            textBoxSeminarfachthemaKurz.Text = seminarfachnoten[0].ThemaKurz;
            textBoxSeminarfachthemaLang.Text = seminarfachnoten[0].ThemaLang;
          }
        }      
    }

    // schreibt eine Notenliste (z.B. alle SA in Englisch aus dem 1. Hj. ins Grid), bez wird als Text an jede Note angefügt
    // gibt die nächste freie Spalte zurück
    private int InsertNoten(int startCol, int startRow, IList<int> noten, string bez="")
    {
        foreach (var note in noten)
        {
                if (bez=="")
                    dataGridNoten.Rows[startRow].Cells[startCol].Value = note;
                else
                    dataGridNoten.Rows[startRow].Cells[startCol].Value = note + "(" + bez + ")";
                startCol++;
        }
        return startCol;
    }

    // schreibt eine Schnittkonstellation ins Grid    
    private void InsertSchnitt(int startCol, int startRow, BerechneteNote b)
    {
        if (b != null)
        {
            dataGridNoten.Rows[startRow].Cells[startCol].Value = b.SchnittSchulaufgaben;
            dataGridNoten.Rows[startRow + 1].Cells[startCol].Value = b.SchnittMuendlich;
            dataGridNoten.Rows[startRow + 1].Cells[startCol + 1].Value = b.JahresfortgangMitKomma;
            dataGridNoten.Rows[startRow].Cells[startCol + 1].Value = b.JahresfortgangGanzzahlig;
        }
    }

        private void buttonSpeichern_Click(object sender, EventArgs e)
    {
      if (schueler != null && schueler.Data.Jahrgangsstufe == "11")
      {
        FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
        var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
        if (fpANoten.Count == 1)
        {
          fpAAdapter.Update(listBoxFpA.SelectedIndex, textBoxFpABemerkung.Text, schueler.Id);
        }
        else
        {
          fpAAdapter.Insert(schueler.Id, listBoxFpA.SelectedIndex, textBoxFpABemerkung.Text);
        }
      }

      if (schueler != null && schueler.Data.Jahrgangsstufe == "13")
      {
        SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
        var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
        if (seminarfachnoten.Count == 1)
        {
          seminarfachAdapter.Update((int)numericUpDownSeminarfach.Value, textBoxSeminarfachthemaLang.Text, textBoxSeminarfachthemaKurz.Text, schueler.Id);
        }
        else
        {
          seminarfachAdapter.Insert(schueler.Id, (int)numericUpDownSeminarfach.Value, textBoxSeminarfachthemaLang.Text, textBoxSeminarfachthemaKurz.Text);
        }
      }
    }

    private void btnSchliessen_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
