using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tracktv.DAO;
using Tracktv.DTO;
using System.Collections.ObjectModel;

namespace TrackTV_WEB.Forms
{
    public partial class MyAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                User u = new User();
                u = UsersTable.login(HttpContext.Current.Session["login"].ToString(), HttpContext.Current.Session["password"].ToString());
                userLogin.InnerText = u.Name;
                dailyscore.InnerText = u.DailyScore.ToString();

                User bestUser = new User();
                bestUser = UsersTable.CalculateDailyScore();
                if(bestUser != null){
                    bestDailyScoreUserLogin.InnerText = bestUser.Login;
                    bestDailyScoreUserScore.InnerText = bestUser.DailyScore.ToString();
                }
                

                UsersTable.getUserDetail(u);
                mostwatchedactor.InnerText = u.mostWatchedActor.Name;
                mostwatchedshow.InnerText = u.mostWatchedShow.Name;
                timesinhistory.InnerText = u.mostWatchedShow.timesInUserHistory.ToString();
                bestcommentText.InnerText = "„" + u.bestComment.Text + "“";
                bestcommentScore.InnerText = "With " + u.bestComment.Score + " points";

                HistoryTable.getUserHistory(u);
                UserHistory.DataSource = u.history;
                UserHistory.DataBind();


            }
            
        }
    }
}