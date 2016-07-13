use [diNo]

select Ausbildungsrichtung,Erfolg,Count(Schueler.Id) as Anzahl
From Schueler,Klasse,FpANoten
Where schueler.KlasseId = Klasse.Id and Schueler.Id = FpANoten.SchuelerId
and Erfolg is not null and left(Bezeichnung,3)='F11'
group by Erfolg, Ausbildungsrichtung

-- für 1. Hj die Variable Erfolg durch Erfolg1Hj ersetzen

