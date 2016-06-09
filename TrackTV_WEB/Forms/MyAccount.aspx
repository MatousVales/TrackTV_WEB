<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MyAccount.aspx.cs" Inherits="TrackTV_WEB.Forms.MyAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      
 <div class="ui raised very padded text container segment">
        
        <h2 class="ui green header">
           <i class="large user middle aligned icon"></i>
          <div class="content">
            Your Account
          </div>
        </h2>
        <h4 class="ui horizontal divider header">
         Statistics are fun!
 
        </h4>
        <br />
     <br />

        <div class="ui grid">
                <div class="eight wide column">

                    <div class="ui centered card">
  <div class="image">
    <img src="~/theme/themes/default/assets/images/steve.jpg" runat="server" />
  </div>
  <div class="content">
    <a class="header" id="userLogin" runat="server"/>
  </div>
</div>



                    </div>

                <div class="eight wide column">
                    <h4 class="ui horizontal divider header">
                  <i class="bar chart icon"></i>
                    Dailyscore
                 </h4>

                    <br />
                    <div class="ui center aligned grid">
                        <div class="sixteen wide column">
                         <div class="green ui statistic">
                            <div class="value" id="dailyscore" runat="server">
                            </div>
                            <div class="label">
                              Points today
                            </div>
                          </div>
                        </div>
                       <div class="sixteen wide column">
                           <br />
                       </div>
                        
                        <div class="eight wide column">
                            <div class="ui green statistic">
                            <div class="text value" id="bestDailyScoreUserLogin" runat="server">
                              Nobody has any score yet
                            </div>
                            <div class="label">
                              Today's best user
                            </div>
                          </div>
                       </div>
                       <div class="eight wide column">
                          <div class="ui green statistic">
                            <div class="value" id="bestDailyScoreUserScore" runat="server">
                                0
                              <i class="trophy icon"></i> 
                            </div>
                            <div class="label">
                              Points
                            </div>
                          </div>
                        </div>
                    
                  </div>
                       
     </div>
  </div>

     <br />
     <h4 class="ui horizontal divider header">
                  <i class="pied piper alternate icon"></i>
                  Your most watched actor:
     </h4>
                <div class="ui center aligned container">
          <br />
            <div class="green ui statistic">
                        <div class="text value" id="mostwatchedactor" runat="server">
                        </div>  
                      </div>
         </div>

   
    <h4 class="ui horizontal divider header">
                  <i class="youtube play icon"></i>
                  Your most watched show:
     </h4>
     <br />
      <div class="ui center aligned grid">
                <div class="eight wide column">
                            <div class="ui green statistic">
                            <div class="text value" id="mostwatchedshow" runat="server">
                            </div>
                            <div class="label">
                              Name
                            </div>
                          </div>
                       </div>
                       <div class="eight wide column">
                          <div class="ui green statistic">
                            <div class="value" id="timesinhistory" runat="server">
                              <i class="repeat icon"></i> 5
                            </div>
                            <div class="label" >
                                Times in history
                            </div>
                          </div>
                        </div>
      </div>



     <h4 class="ui horizontal divider header">
                  <i class="comments outline icon"></i>
                  Your best comment:
                 </h4>
    
     <div class="ui center aligned container">
           <br />
     <div class="green ui statistic">
                        <div class="text value" id="bestcommentText" runat="server">
                          	
                        </div>
                             <div class="label" id="bestcommentScore" runat="server">
                                
                            </div>
                        
                      </div>

    </div>









      <h4 class="ui horizontal divider header">
                  <i class="history icon"></i>
                  Your history
                 </h4>
     <div class="ui relaxed divided list">
                             <asp:Repeater ID="UserHistory" runat="server">
                            <ItemTemplate>
                                  <div class="ui green segment">
                                      <div class="ui grid">
                                          <div class="one wide column">
                                              <i class="large film middle aligned icon"></i>
                                            </div>
                                          <div class="five wide column">
                                              <%# Eval("showName") %>
                                          </div>
                                          <div class="five wide column">
                                              <%# Eval("episodeName") %>
                                          </div>
                                          <div class="five wide column">
                                              <%# Eval("Datetime") %>
                                          </div>      
                                  </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>   
                        </div> 

    </div>

</asp:Content>

