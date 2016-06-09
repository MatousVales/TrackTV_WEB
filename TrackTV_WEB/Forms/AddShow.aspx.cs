using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tracktv.DAO;
using Tracktv.DTO;
using System.Collections.ObjectModel;
using System.Web.UI.HtmlControls;

namespace TrackTV_WEB.Forms
{
    public partial class AddShow : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Collection<Show> shows = ShowsTable.getListing();
                ShowNames.DataSource = shows;
                ShowNames.DataBind();

                string qS;
                int sID;
                qS = Request.QueryString["sID"];

                if (qS != null)
                {
                    sID = Convert.ToInt32(qS);
                    foreach (Show s in shows)
                    {
                        if (s.sID == sID)
                        {
                            ShowsTable.getListingDetail(s);
                            show_name.Value = s.Name;
                            show_director.Value = s.Director;
                            show_goldenglobe.Value = s.hasGoldenGlobe.ToString();
                            show_genre.Value = s.Genre;
                        }
                    }
                }
            }
        }


        protected void show_name_btn_OnClick(object Source, EventArgs e)
        {
            string queryID = Request.QueryString["sID"];
            Show s = new Show();
            s.sID = Convert.ToInt32(queryID);
            ShowsTable.updateName(s, show_name.Value);

            Collection<Show> newShows = ShowsTable.getListing();
            ShowNames.DataSource = newShows;
            ShowNames.DataBind();
        }

        protected void show_director_btn_OnClick(object Source, EventArgs e)
        {
            string queryID = Request.QueryString["sID"];
            Show s = new Show();
            s.sID = Convert.ToInt32(queryID);
            ShowsTable.updateDirector(s, show_director.Value);
        }

        protected void show_goldenglobe_btn_OnClick(object Source, EventArgs e)
        {
            string queryID = Request.QueryString["sID"];
            int sID = Convert.ToInt32(queryID);
            ShowsTable.awardGoldenGlobe(sID);
        }

        protected void show_genre_btn_OnClick(object Source, EventArgs e)
        {
            string queryID = Request.QueryString["sID"];
            Show s = new Show();
            s.sID = Convert.ToInt32(queryID);
            ShowsTable.updateGenre(s, show_genre.Value);
        }

        protected void insert_btn_OnClick(object Source, EventArgs e)
        {
            Show s = new Show();
            s.Name = show_name.Value;
            s.Director = show_director.Value;
            if(show_goldenglobe.Value == "True")
            {
                s.hasGoldenGlobe = true;
            } else
            {
                s.hasGoldenGlobe = false;
            }
            s.Genre = show_genre.Value;
            ShowsTable.insert(s);

            Collection<Show> newShows = ShowsTable.getListing();
            ShowNames.DataSource = newShows;
            ShowNames.DataBind();
            Response.Redirect("\\Forms\\AddShow.aspx?sID=" + s.sID);
        }
    }
}