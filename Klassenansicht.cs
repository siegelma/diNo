﻿using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace diNo
{
  public partial class Klassenansicht : Form
  {
    public Klassenansicht()
    {
      InitializeComponent();

      this.olvColumnBezeichnung.AspectGetter = KlassenTreeViewController.SelectValueCol1;
      Generator.GenerateColumns(this.objectListView1, typeof(Schueler), false);
    }

    private void treeListView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      var schueler = treeListView1.SelectedObject as Schueler;
      if (schueler != null)
      {
        this.btnNotenbogenZeigen.Enabled = true;
        this.objectListView1.Objects = new List<Schueler>() { schueler };
      }
      else
      {
        this.btnNotenbogenZeigen.Enabled = false;
      }
    }

    private void btnNotenbogenZeigen_Click(object sender, EventArgs e)
    {
      var schueler = treeListView1.SelectedObject as Schueler;
      if (schueler != null)
      {
        Notenbogen bogen = new Notenbogen(schueler.Id);
        bogen.Show();
      }
    }

    private void Klassenansicht_Load(object sender, EventArgs e)
    {
      this.treeListView1.Roots = KlassenTreeViewController.GetSortedKlassenList(true);
      this.treeListView1.CanExpandGetter = delegate (object x) { return (x is Klasse); };
      this.treeListView1.ChildrenGetter = delegate (object x) { return KlassenTreeViewController.GetSortedSchuelerList((Klasse)x); };
    }
  }
}
