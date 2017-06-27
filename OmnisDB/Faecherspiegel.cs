﻿using log4net;
using System;
using System.Globalization;

namespace diNo.OmnisDB
{
  public class Faecherspiegel
  {
    // da bei Reli und Ethik sowieso immer nachgeschaut werden muss, ob der Schüler dieses Fach auch wirklich belegt
    // müssen diese beiden (Index=0 und Index=1) sowieso immer gesondert betrachtet werden

    private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private OmnisConnection omnis;

    public Faecherspiegel()
    {
      this.omnis = new OmnisConnection();
    }

    /// <summary>
    /// Sucht das Fach am angegebenen Index aus dem angegebenen Fächerspiegel.
    /// </summary>
    /// <param name="faecherspiegel">Welcher Fächerspiegel, z. B. W11.</param>
    /// <param name="index">Der Index des Faches.</param>
    /// <param name="schulart">FOS oder BOS.</param>
    /// <param name="schueler">Den Schüler brauchen wir auch, um zu ermitteln ob er katholisch oder evangelisch ist.</param>
    /// <param name="zeitpunkt">Der Zeitpunkt, für welchen wir die Note ermitteln müssen.</param>
    /// <returns>Das Fach oder null, wenn kein weiteres Fach mehr im Fächerspiegel vorhanden ist.</returns>
    public string GetFachNoteString(string faecherspiegel, int index, Schulart schulart, Schueler schueler, Zeitpunkt zeitpunkt)
    {
      string faecherKuerzel = omnis.SucheFach(faecherspiegel, index, schulart); // hier nur zur Anzeige etwaiger Fehlermeldungen benötigt
      if (string.IsNullOrEmpty(faecherKuerzel))
      {
        return ""; // Wenn kein sinnvolles Fach mehr kommt, bleibt das Notenfeld leer
      }

      var dieRichtigeNote = FindeFachNoten(faecherKuerzel, schueler);
      if (dieRichtigeNote == null)
      {
        if (FehlendeNoteWirdWohlOKSein(faecherKuerzel) || !schueler.Data.IsAustrittsdatumNull())
        {
          log.Debug(schueler.NameVorname + " sollte in " + faecherKuerzel + " gehen, aber diese Zuordnung findet diNo nicht!");
        }
        else
        {
          log.Warn(schueler.NameVorname + " sollte in " + faecherKuerzel + " gehen, aber diese Zuordnung findet diNo nicht!");
        }
        return "-";
      }
      else
      {
        return GetNotenString(dieRichtigeNote, zeitpunkt);
      }
    }

    public string SucheFach(string faecherspiegel, int index, Schulart schulart)
    {
      return omnis.SucheFach(faecherspiegel, index, schulart);
    }

    public string FindeJahresfortgangsNoten(string faecherspiegel, int index, Schulart schulart, Schueler schueler, Zeitpunkt zeitpunkt)
    {
      string faecherKuerzel = SucheFach(faecherspiegel, index, schulart); // hier nur zur Anzeige etwaiger Fehlermeldungen benötigt
      if (string.IsNullOrEmpty(faecherKuerzel))
      {
        return "";
      }

      var noten = FindeFachNoten(faecherKuerzel, schueler);
      if (noten == null)
      {
        return "-";
      }

      BerechneteNote note = noten.getSchnitt(Halbjahr.Zweites);
      // wenn alle Noten leer sind => Note liegt nicht vor. Entwerte Fach in WinSV.
      if (note.JahresfortgangMitKomma == null && note.JahresfortgangGanzzahlig == null)
      {
        return "-";
      }
      // wenn der Jahresfortgang(Komma) leer ist aber eine Gesamtnote existiert, dann nimm diese (vermutlich G, TZ usw.)
      decimal? nimmNote = note.JahresfortgangMitKomma != null ? note.JahresfortgangMitKomma : note.JahresfortgangGanzzahlig;
      if (nimmNote == null)
      {
        log.Warn("nicht vorliegende Note im Fach "+faecherKuerzel + " bei Schüler "+schueler.NameVorname);
        return "-";
      }

      return string.Format(CultureInfo.CurrentCulture, "{0:00.00}", nimmNote);
    }

    public string FindeAPSchriftlichNoten(string faecherspiegel, int index, Schulart schulart, Schueler schueler, Zeitpunkt zeitpunkt)
    {
      string faecherKuerzel = omnis.SucheFach(faecherspiegel, index, schulart); // hier nur zur Anzeige etwaiger Fehlermeldungen benötigt
      if (string.IsNullOrEmpty(faecherKuerzel))
      {
        return "";
      }

      var noten = FindeFachNoten(faecherKuerzel, schueler);
      if (noten == null)
      {
        return "-"; // Fach wurde wohl nicht belegt (kann ab und zu vorkommen, z. B. bei Wahlfächern)
      }

      var apnote = noten.getNoten(Halbjahr.Zweites, Notentyp.APSchriftlich);
      if (apnote == null || apnote.Count == 0)
      {
        return "";
      }

      return string.Format(CultureInfo.CurrentCulture, "{0:00}", apnote[0]);
    }

    public string FindeAPMuendlichNoten(string faecherspiegel, int index, Schulart schulart, Schueler schueler, Zeitpunkt zeitpunkt)
    {
      string faecherKuerzel = omnis.SucheFach(faecherspiegel, index, schulart); // hier nur zur Anzeige etwaiger Fehlermeldungen benötigt
      if (string.IsNullOrEmpty(faecherKuerzel))
      {
        return "";
      }

      var noten = FindeFachNoten(faecherKuerzel, schueler);
      if (noten == null)
      {
        return "-"; // Fach wurde wohl nicht belegt (kann ab und zu vorkommen, z. B. bei Wahlfächern)
      }

      var apnote = noten.getNoten(Halbjahr.Zweites, Notentyp.APMuendlich);
      if (apnote == null || apnote.Count == 0)
      {
        return "";
      }

      return string.Format(CultureInfo.CurrentCulture, "{0:00}", apnote[0]);
    }

    public FachSchuelerNoten FindeFachNoten(string faecherKuerzel, Schueler schueler)
    {
      if (faecherKuerzel.Equals("Rel", StringComparison.OrdinalIgnoreCase))
      {
        // Relinote - je nachdem, ob Schüler Evangelisch oder Katholisch ist. Geht er in Ethik: "-"
        var kath = schueler.getNoten.FindeFach("K", false);
        if (kath != null)
          return kath;

        var ev = schueler.getNoten.FindeFach("Ev", false);
        if (ev != null)
          return ev;

        return null; //offenbar weder kath. noch ev.
      }
      if (faecherKuerzel.Equals("Eth", StringComparison.OrdinalIgnoreCase))
      {
        //Ethiknote (wenn der Schüler in Ethik geht, sonst null)
        return schueler.getNoten.FindeFach("Eth", false);
      }

      return schueler.getNoten.FindeFach(faecherKuerzel, false);
    }

    public bool FehlendeNoteWirdWohlOKSein(string faecherkuerzel)
    {
      var fach = faecherkuerzel.ToUpper();
      return fach == "F-WI" || fach == "MU" || fach == "WIN" || fach == "REL" || fach == "ETH";
    }

    public string GetNotenString(FachSchuelerNoten note, Zeitpunkt zeitpunkt)
    {

      byte? relevanteNote = note.getRelevanteNote(zeitpunkt); 
      return relevanteNote == null ? "-" : string.Format("{0:00}", (byte)relevanteNote); //wichtig: Im Feld muss 08 stehen, nicht 8 (d.h. 2 Ziffern)
    }
  }
}
