/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
use diNo;

SELECT Lehrer.Nachname, Lehrer.Kuerzel, Rolle.Bezeichnung
FROM LehrerRolle, Lehrer, Rolle
Where Lehrer.Id=LehrerRolle.LehrerId and Rolle.Id = LehrerRolle.RolleId
order by Lehrer.Nachname, Lehrer.Vorname