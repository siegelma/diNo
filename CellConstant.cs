﻿using System;
namespace diNo
{
  /// <summary>
  /// Enthält die Konstanten, in welcher Zelle welche Noten stehen.
  /// </summary>
  internal static class CellConstant
  {
    public static string Klassenbezeichnung = "B1";
    public static string Wertungsart = "C1";
    public static string Fachbezeichnung = "F1";
    public static string Lehrer = "M1";
    public static string Schuljahr = "S1";
    public static string DatumStand = "Z1";
    public static int ZeileErsterSchueler = 5;
    
    #region Konstanten geben das Feld für den ersten Schüler an.
    
    public static string Nachname = "B";
    public static string LegasthenieVermerk = "Y";
    public static string LegasthenieEintragung = "Legasthenie";


    #endregion

    #region Ab hier die untere Zeile der beiden Zeilen eines Schuelers.

    public static string Vorname = "B";

    #endregion

    public static int GewichteSchulaufgaben = 75;
    public static int GewichteExen = 76;

    #region AP

 public static int APZeileErsterSchueler = 6;

    #endregion

    #region diNo
    /// <summary>
    /// Achtung diese Konstanten liegen auf einem anderen Datenblatt "diNo".
    /// </summary>
    public static string SId = "C";
    public static int zeileSIdErsterSchueler = 4;
    public static string KursId = "F2";

    #endregion

    #region Notenschluessel
    /// <summary>
    /// M oder E zulässig
    /// </summary>
    public static string SchluesselArt ="H32";

    public static string ProzentFuenfUntergrenze = "H34";
    public static string ProzentFuenfObergrenze = "H35";

        #endregion

        /// <summary>
        /// liefert zum angegeben Notentyp und Halbjahr die Spalte im Excelsheet. Zusammen mit der Zeile (=obere Zeile) 
        /// des Schülers wird die Zelle generiert.
        /// </summary>
        public static string[] getLNWZelle(Notentyp typ, Halbjahr hj,int zeile)
        {
            string[] s = new string[] { };            

            if (hj == Halbjahr.Erstes)
            {
                zeile++; // die meisten Noten stehen unten
                switch (typ)
                {                    
                    case Notentyp.Schulaufgabe: s =  new[] { "D", "E", "F" }; zeile--; break;
                    case Notentyp.Ex: s =  new[] { "D", "E", "F", "G" };  break;
                    case Notentyp.EchteMuendliche: s =  new[] { "H", "I", "J" }; break;
                    case Notentyp.Fachreferat: s =  new[] { "L" }; break;
                    case Notentyp.Ersatzprüfung: s =  new[] { "K" }; break;
                }
            }

            if (hj == Halbjahr.Zweites)
            {
                zeile++;
                switch (typ)
                {                   
                    case Notentyp.Schulaufgabe: s =  new[] { "P", "Q", "R" }; zeile--; break;
                    case Notentyp.Ex: s =  new[] { "P", "Q", "R", "S" }; break;
                    case Notentyp.EchteMuendliche: s =  new[] { "T", "U", "V" }; break;
                    case Notentyp.Fachreferat: s =  new[] { "X" }; break;
                    case Notentyp.Ersatzprüfung: s =  new[] { "W" }; break;
                    case Notentyp.APSchriftlich: s = new[] { "E" }; break;
                    case Notentyp.APMuendlich: s = new[] { "F" }; break;
                }
            }
            
            for (int i = 0; i < s.Length; i++)
                s[i] = s[i] + zeile;     // Zeilennummer an jede Spalte anhängen

            return s; // ggf. eine leere Liste, falls diese Kombi nicht zulässig ist
        }

        /// <summary>
        /// liefert zum angegeben Notentyp und Halbjahr die Spalte im Excelsheet. Zusammen mit der Zeile (=obere Zeile) 
        /// des Schülers wird die Zelle generiert.
        /// </summary>
        public static string getSchnittZelle(BerechneteNotentyp typ, Halbjahr hj, int zeile)
        {
            string s=null;

            if (hj == Halbjahr.Erstes)
            {
                zeile++; // die meisten Noten stehen unten
                switch (typ)
                {
                    case BerechneteNotentyp.SchnittSA: s = "M"; break;
                    case BerechneteNotentyp.Schnittmuendlich: s = "N"; break;
                    case BerechneteNotentyp.JahresfortgangMitNKS: s = "O"; break;
                    case BerechneteNotentyp.Jahresfortgang: s = "O"; zeile--; break;
                }
            }

            if (hj == Halbjahr.Zweites)
            {
                zeile++;
                switch (typ)
                {
                    case BerechneteNotentyp.SchnittSA: s = "Y"; break;
                    case BerechneteNotentyp.Schnittmuendlich: s = "Z"; break;
                    case BerechneteNotentyp.JahresfortgangMitNKS: s = "AA"; break;
                    case BerechneteNotentyp.Jahresfortgang: s = "AA"; zeile--; break;
                    case BerechneteNotentyp.APGesamt: s = "G"; break;
                    case BerechneteNotentyp.Abschlusszeugnis: s ="I"; break;
                }
            }
            if (s != null) s = s + zeile;
            return s; // ggf. ein null String, falls diese Kombi nicht zulässig ist
        }

    }

}

