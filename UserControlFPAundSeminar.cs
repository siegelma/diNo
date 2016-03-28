﻿using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlFPAundSeminar : UserControl
  {
    private Schueler schueler;

    public UserControlFPAundSeminar()
    {
      InitializeComponent();
      cbBetreuer.BeginUpdate();
      cbBetreuer.DataSource = Zugriff.Instance.Lehrerliste.ToList();
      cbBetreuer.DisplayMember = "Value";
      cbBetreuer.ValueMember = "Key";
      cbBetreuer.EndUpdate();
    }

    public Schueler Schueler
    {
      get
      {
        return schueler;
      }
      set
      {
        this.schueler = value;
        if (this.schueler != null)
        {
          Init();
        }
      }
    }

    private bool IstFpaAenderbar()
    {
      if (schueler.getKlasse.Jahrgangsstufe != Jahrgangsstufe.Elf)
      {
        return false;
      }

      if (schueler.BetreuerId == Zugriff.Instance.lehrer.Id || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
      {
        return true;
      }

      return (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAAgrar) && schueler.Zweig == Zweig.Agrar) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpASozial) && schueler.Zweig == Zweig.Sozial) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpATechnik) && schueler.Zweig == Zweig.Technik) ||
        (Zugriff.Instance.lehrer.HatRolle(Rolle.FpAWirtschaft) && schueler.Zweig == Zweig.Wirtschaft);
    }

    public void Init()
    {
      pnlBetreuer.Enabled = (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf || schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn) && Zugriff.Instance.lehrer.HatRolle(Rolle.Admin);
      cbBetreuer.SelectedValue = schueler.Data.IsBetreuerIdNull() ? -1 : schueler.Data.BetreuerId;
      pnlFPA.Enabled = IstFpaAenderbar();
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
      {
        pnlFPA.Enabled = true;
        var fpANoten = schueler.FPANoten;
        textBoxFpABemerkung.Text = fpANoten.IsBemerkungNull() ? "" : fpANoten.Bemerkung;
        cbFPAErfolg1Hj.SelectedIndex = fpANoten.IsErfolg1HjNull() ? 0 : fpANoten.Erfolg1Hj;
        cbFPAErfolg.SelectedIndex = fpANoten.IsErfolgNull() ? 0 : fpANoten.Erfolg;
        numPunkte.Value = fpANoten.IsPunkteNull() ? null : (decimal?)fpANoten.Punkte;
        numPunkte1Hj.Value = fpANoten.IsPunkte1HjNull() ? null : (decimal?)fpANoten.Punkte1Hj;
        numPunkte2Hj.Value = fpANoten.IsPunkte2HjNull() ? null : (decimal?)fpANoten.Punkte2Hj;
      }
      else
      {
        pnlFPA.Enabled = false;
        textBoxFpABemerkung.Text = "";
        cbFPAErfolg1Hj.SelectedIndex = 0;
        cbFPAErfolg.SelectedIndex = 0;
        numPunkte.Value = null;
        numPunkte1Hj.Value = null;
        numPunkte2Hj.Value = null;
      }

      pnlSeminar.Enabled = schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn && (Zugriff.Instance.lehrer.HatRolle(Rolle.Seminarfach) || Zugriff.Instance.lehrer.HatRolle(Rolle.Admin) || Zugriff.Instance.lehrer.Id == schueler.Data.BetreuerId);
      if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
      {
        var sem = schueler.Seminarfachnote;
        numSeminarpunkte.Value = sem.IsGesamtnoteNull() ? null : (decimal?)sem.Gesamtnote;
        textBoxSeminarfachthemaKurz.Text = sem.IsThemaKurzNull() ? "" : sem.ThemaKurz;
        textBoxSeminarfachthemaLang.Text = sem.IsThemaLangNull() ? "" : sem.ThemaLang;

      }
      else
      {
        numSeminarpunkte.Value = null;
        textBoxSeminarfachthemaKurz.Text = "";
        textBoxSeminarfachthemaLang.Text = "";
      }
    }

    public void DatenUebernehmen()
    {
      var fpANoten = schueler.FPANoten;
      if (textBoxFpABemerkung.Text == "") fpANoten.SetBemerkungNull(); else fpANoten.Bemerkung = textBoxFpABemerkung.Text;
      if (cbFPAErfolg1Hj.SelectedIndex == 0) fpANoten.SetErfolg1HjNull(); else fpANoten.Erfolg1Hj = cbFPAErfolg1Hj.SelectedIndex;
      if (cbFPAErfolg.SelectedIndex == 0) fpANoten.SetErfolgNull(); else fpANoten.Erfolg = cbFPAErfolg.SelectedIndex;
      if (numPunkte.Value == null) fpANoten.SetPunkteNull(); else fpANoten.Punkte = (int)numPunkte.Value;
      if (numPunkte1Hj.Value == null) fpANoten.SetPunkte1HjNull(); else fpANoten.Punkte1Hj = (int)numPunkte1Hj.Value;
      if (numPunkte2Hj.Value == null) fpANoten.SetPunkte2HjNull(); else fpANoten.Punkte2Hj = (int)numPunkte2Hj.Value;

      var sem = schueler.Seminarfachnote;
      if (numSeminarpunkte.Value == null) sem.SetGesamtnoteNull(); else sem.Gesamtnote = (int)numSeminarpunkte.Value;
      if (textBoxSeminarfachthemaKurz.Text == "") sem.SetThemaKurzNull(); else sem.ThemaKurz = textBoxSeminarfachthemaKurz.Text;
      if (textBoxSeminarfachthemaLang.Text == "") sem.SetThemaLangNull(); else sem.ThemaLang = textBoxSeminarfachthemaLang.Text;

      if (Zugriff.Instance.lehrer.HatRolle(Rolle.Admin))
      {
        if (cbBetreuer.SelectedValue == null) schueler.Data.SetBetreuerIdNull();
        else schueler.Data.BetreuerId = (int)cbBetreuer.SelectedValue;
      }
    }
  }
}