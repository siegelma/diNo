
Use diNo
Alter Table Rolle Add KlassenString nvarchar(6) null;
go

update Rolle set KlassenString = '%13%' Where Id = 4
update Rolle set KlassenString = 'F11W%' Where Id = 5
update Rolle set KlassenString = 'F11S%' Where Id = 6
update Rolle set KlassenString = 'F11T%' Where Id = 7
update Rolle set KlassenString = 'F11A%' Where Id = 8
