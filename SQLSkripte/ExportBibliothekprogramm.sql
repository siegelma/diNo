use diNo;

SELECT
      [Name]
      ,[Rufname] AS Vorname
      ,[AnschriftPLZ]
      ,[AnschriftOrt]
      ,[AnschriftStrasse]
      ,[AnschriftTelefonnummer]
	  ,[Geschlecht]
	  ,[Bezeichnung] AS Klasse
      ,[Ausbildungsrichtung] AS Zweig
  FROM [dbo].[Schueler],[dbo].[Klasse]
  WHERE Schueler.KlasseId = Klasse.Id 
  AND (Status=0)

  ORDER BY Bezeichnung,Ausbildungsrichtung, Name