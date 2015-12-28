﻿using diNo.diNoDataSetTableAdapters;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class Notenbogen : UserControl
  {
    private Schueler schueler;
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// leerer Konstruktor ist Pflicht für UserControls
    /// </summary>
    public Notenbogen()
    {
      InitializeComponent();
      dataGridNoten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // sollte das Neuzeichnen schneller machen
    }

    /// <summary>
    /// Der anzuzeigende Schüler.
    /// </summary>
    public Schueler Schueler
    {
      get
      {
        return this.schueler;
      }
      set
      {
        this.schueler = value;
        this.Init();
      }
    }

    /// <summary>
    /// Füllt die Felder mit den Daten des Schülers.
    /// </summary>
    public void Init()
    {
      if (this.schueler == null)
      {
        return;
      }

      dataGridNoten.Rows.Clear();
      dataGridNoten.RowsDefaultCellStyle.BackColor = Color.White;
      log.Debug("Öffne Notenbogen SchülerId=" + this.schueler.Id);

      var dieNoten = schueler.getNoten.alleFaecher;
      if (dieNoten.Count == 0)
      {
        // alle Kurszuordnungen des Schülers wurden gelöscht. Vermutlich schauen wir auf einen ausgetretenen Schüler.
        // in diesem Fall suche die Noten anhand der möglichen Fächer, trage diese ein und graue den Notenbogen aus
        dieNoten = schueler.getNoten.SucheAlteNoten();
        dataGridNoten.RowsDefaultCellStyle.BackColor = Color.LightGray;
      }

      // Row[lineCount] für schriftliche und Row[lineCount+1] für mündliche Noten
      int lineCount = 0;
      foreach (var kursNoten in dieNoten)
      {
        dataGridNoten.Rows.Add(2);
        dataGridNoten.Rows[lineCount + 1].Height += 2;
        dataGridNoten.Rows[lineCount + 1].DividerHeight = 2;
        dataGridNoten.Rows[lineCount].Cells[0].Value = kursNoten.getFach.Bezeichnung;

        InsertNoten(1, lineCount, kursNoten.getNoten(Halbjahr.Erstes, Notentyp.Schulaufgabe));
        InsertNoten(1, lineCount + 1, kursNoten.sonstigeLeistungen(Halbjahr.Erstes));

        InsertNoten(10, lineCount, kursNoten.getNoten(Halbjahr.Zweites, Notentyp.Schulaufgabe));
        InsertNoten(10, lineCount + 1, kursNoten.sonstigeLeistungen(Halbjahr.Zweites));

        InsertSchnitt(8, lineCount, kursNoten.getSchnitt(Halbjahr.Erstes));
        BerechneteNote zeugnis = kursNoten.getSchnitt(Halbjahr.Zweites);
        InsertSchnitt(18, lineCount, zeugnis);

        InsertNoten(20, lineCount, kursNoten.getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich));
        InsertNoten(20, lineCount + 1, kursNoten.getNoten(Halbjahr.Zweites, Notentyp.APMuendlich));

        if (zeugnis != null)
        {
          if (zeugnis.PruefungGesamt != null)
          {
            dataGridNoten.Rows[lineCount].Cells[21].Value = zeugnis.PruefungGesamt;
            dataGridNoten.Rows[lineCount].Cells[22].Value = zeugnis.SchnittFortgangUndPruefung;
            dataGridNoten.Rows[lineCount].Cells[23].Value = zeugnis.Abschlusszeugnis;
          }
          else
            dataGridNoten.Rows[lineCount].Cells[23].Value = zeugnis.JahresfortgangGanzzahlig;
        }
        lineCount = lineCount + 2;
      }

      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {
        FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
        var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
        if (fpANoten.Count == 1)
        {
          textBoxFpABemerkung.Text = fpANoten[0].Bemerkung;
          listBoxFpA.SelectedIndex = fpANoten[0].Note;
        }
      }

      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
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
    private void InsertNoten(int startCol, int startRow, IList<string> noten)
    {
      foreach (var note in noten)
      {
        var cell = dataGridNoten.Rows[startRow].Cells[startCol];
        cell.Value = note;
      //  SetBackgroundColor(note, cell);
        startCol++;
      }
    }

    private void InsertNoten(int startCol, int startRow, IList<int> noten)
    {
      foreach (var note in noten)
      {
        var cell = dataGridNoten.Rows[startRow].Cells[startCol];
        cell.Value = note;
       // SetBackgroundColor(note, cell);
        startCol++;
      }
    }

    // schreibt eine Schnittkonstellation ins Grid    
    private void InsertSchnitt(int startCol, int startRow, BerechneteNote b)
    {
      if (b != null)
      {
        if (b.SchnittSchulaufgaben != null)
        {
          var cell = dataGridNoten.Rows[startRow].Cells[startCol];
          cell.Value = b.SchnittSchulaufgaben;
          // SetBackgroundColor((double)b.SchnittSchulaufgaben, cell);
        }

        if (b.SchnittMuendlich != null)
        {
          dataGridNoten.Rows[startRow + 1].Cells[startCol].Value = b.SchnittMuendlich;
          // SetBackgroundColor((double)b.SchnittMuendlich, dataGridNoten.Rows[startRow + 1].Cells[startCol]);
        }

        if (b.JahresfortgangMitKomma != null)
        {
          dataGridNoten.Rows[startRow + 1].Cells[startCol + 1].Value = b.JahresfortgangMitKomma;
          SetBackgroundColor((double)b.JahresfortgangMitKomma, dataGridNoten.Rows[startRow + 1].Cells[startCol + 1]);
        }

        if (b.JahresfortgangGanzzahlig != null)
        {
          dataGridNoten.Rows[startRow].Cells[startCol + 1].Value = b.JahresfortgangGanzzahlig;
          SetBackgroundColor((double)b.JahresfortgangGanzzahlig, dataGridNoten.Rows[startRow].Cells[startCol + 1]);
        }
      }
    }

    private void SetBackgroundColor(string note, DataGridViewCell cell)
    {
      
      double notenwert = double.MaxValue;
      if (!double.TryParse(note, out notenwert))
      {
        return; // nur zur Sicherheit: Wenn das parsen nicht klappt, muss man ja nicht gleich abstürzen
      }

      SetBackgroundColor(notenwert, cell);
    }

    private void SetBackgroundColor(double notenwert, DataGridViewCell cell)
    {
      Color color = GetBackgroundColor(notenwert);
      if (color == dataGridNoten.BackgroundColor)
      {
        return; // nothing to do
      }
      else
      {
        cell.Style.BackColor = color;
      }
    }

    private Color GetBackgroundColor(double notenwert)
    {
      if (notenwert < 1) return Color.Red;
      if (notenwert < 1.5) return Color.OrangeRed;
      if (notenwert < 2.5) return Color.DarkOrange;
      if (notenwert < 3.5) return Color.Orange;
      if (notenwert < 4.5) return Color.Yellow;
      if (notenwert < 5.5) return Color.LightYellow;

      if (notenwert > 13.5) return Color.Green;
      if (notenwert > 11.5) return Color.LightGreen;
      return dataGridNoten.BackgroundColor;
    }

        private void buttonSpeichern_Click(object sender, EventArgs e)
    {
      
      if (schueler != null && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
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

      if (schueler != null && schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
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

    private void btnDruck_Click(object sender, EventArgs e)
    {
        new ReportNotenbogen(schueler);
    }
  }
}
