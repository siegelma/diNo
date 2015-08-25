﻿using System;
using System.Linq;
using System.Windows.Forms;

using diNo.diNoDataSetTableAdapters;
using System.Collections.Generic;
using log4net;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;

namespace diNo
{

	public partial class Form1 : Form
	{
    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public Form1()
		{
			InitializeComponent();
		}

    private void btnReadLehrer_Click(object sender, EventArgs e)
    {
      LehrerFileReader.Read("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\STDLEHR.txt");
    }

    private static List<KursplanZeile> FilterKurse(IList<KursplanZeile> alleKurseRaw)
    {
      var alleKurse = new List<KursplanZeile>();
      // für Kurse ohne namen vergibt selbst einen
      foreach (var kurs in alleKurseRaw)
      {
        if (string.IsNullOrEmpty(kurs.KursBezeichnung))
        {
          kurs.KursBezeichnung = kurs.FachKurzbez + kurs.Klasse;
        }

        // Todo: Was für Kurse kommen hier weg?
        if (kurs.Klasse != "")
        {
          alleKurse.Add(kurs);
        }
      }
      return alleKurse;
    }

    private void btnReadExcelFile_Click(object sender, EventArgs e)
    {            
      foreach (string file in Directory.GetFiles(Konstanten.ExcelPfad + "\\Abgabe"))
      {
            var reader = new LeseNotenAusExcel(file);
            if (reader.success)
                Directory.Move(file, Konstanten.ExcelPfad + "\\Archiv");
        //notenReader.OnStatusChange += notenReader_OnStatusChange;
        //notenReader.Synchronize(file);
      }
     
    }

    /*
    void notenReader_OnStatusChange(Object sender, NotenAusExcelReader.StatusChangedEventArgs e)
    {
      this.textBoxStatusMessage.Text = e.Status;
    }
    */

    private void btnCreateExcels_Click(object sender, EventArgs e)
    {
            new ErzeugeAlleExcelDateien();
    }

    private void btnReadExcelKurse_Click(object sender, EventArgs e)
    {
      SchuelerKursSelectorHolder kursSelector = new SchuelerKursSelectorHolder();
      kursSelector.AddSelector(new FremdspracheSelector());
      kursSelector.AddSelector(new ReliOderEthikSelector());
      UnterrichtExcelReader.ReadUnterricht("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\Daten_Stani.xlsx", kursSelector);
    }

    private void btnImportSchueler_Click(object sender, EventArgs e)
    {
      WinSVSchuelerReader.ReadSchueler("C:\\Projects\\diNo\\Grunddaten_Notenprogramm\\Datenquelle_WINSV.txt");
    }

    private void button1_Click(object sender, EventArgs e)
    {
      new Klassenansicht().Show();
    }

    private Zeitpunkt GetZeitpunkt()
    {
      string reason = (string)comboBoxZeitpunkt.SelectedItem;
      switch (reason)
      {
        case "Probezeit BOS": return Zeitpunkt.ProbezeitBOS;
        case "Halbjahr": return Zeitpunkt.HalbjahrUndProbezeitFOS;
        case "1. PA": return Zeitpunkt.ErstePA;
        case "2. PA": return Zeitpunkt.ZweitePA;
        case "3. PA": return Zeitpunkt.DrittePA;
        case "Jahresende": return Zeitpunkt.Jahresende;
        default: return Zeitpunkt.None;
      }
    }

    /// <summary>
    /// Führt alle Notenprüfungen durch.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button3_Click(object sender, EventArgs e)
    {
        var contr = new NotenCheckController(GetZeitpunkt());
        //contr.CheckAll();
        contr.CheckKlasse(new Klasse(15)); // 11Te
        new ReportNotencheck(contr.res);

/*
        UserControlChecks printControl = new UserControlChecks();
        printControl.Show();
        // TODO: Das geht hier natürlich noch wesentlich schicker
        printControl.Print(contr.PrintResults());      
*/
    }

    private void btnFixstand_Click(object sender, EventArgs e)
    {
      //TODO: method unchecked
      Zeitpunkt reason = GetZeitpunkt();
      var noteAdapter = new NoteTableAdapter();
      var fixNoteAdapter = new BerechneteNoteTableAdapter();
      var alleNotenDerSchule = fixNoteAdapter.GetData();
      foreach (var note in alleNotenDerSchule)
      {
        fixNoteAdapter.Insert(note.SchnittMuendlich, note.SchnittSchulaufgaben, note.JahresfortgangMitKomma,
          note.JahresfortgangGanzzahlig, note.PruefungGesamt, note.SchnittFortgangUndPruefung, note.Abschlusszeugnis,
          (int)reason, true, note.SchuelerId, note.KursId, note.ErstesHalbjahr);
      }
    }

        private void btnReport_Click(object sender, EventArgs e)
        {
            //new ReportController(Berichtsliste.rptSchuelerliste);
            //new ReportController(Berichtsliste.rptLehrerliste);
            //new ReportFachliste();
            new ReportNotenbogen(null);
        }
    }
}
