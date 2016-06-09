<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AddShow.aspx.cs" Inherits="TrackTV_WEB.Forms.AddShow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="ui raised very padded text container segment">
        
        <h2 class="ui blue header">
           <i class="large desktop middle aligned icon"></i>
          <div class="content">
            Add Shows
          </div>
        </h2>
        <h4 class="ui horizontal divider header">
         Select show from the list to update it's attributes
 
        </h4>
        <br />

            <div class="ui grid">
                <div class="eight wide column">

                    <div class="ui left aligned container">
                         <div class="ui relaxed divided list">
                             <asp:Repeater ID="ShowNames" runat="server">
                            <ItemTemplate>
                                <div class="item">
                                    <i class="large film middle aligned icon"></i>
                                    <div class="content">
                                      <a class="header" href="/Forms/AddShow.aspx?sID=<%# Eval("sID") %>"> <%# Eval("Name") %></a>
                                    </div>
                                </div> 
                            </ItemTemplate>
                        </asp:Repeater>
                             
                        </div>
                    </div>

                </div>

                 <div class="eight wide column">
                     <div class="ui text container">
                        <div class="ui action input">
                              <input type="text" id="show_name" runat="server" />
                              <button class="ui blue right labeled icon button" OnServerClick="show_name_btn_OnClick" id="show_name_btn" runat="server">
                                <i class="film icon"></i>
                                Set
                              </button>
                         </div>
                         <br />
                         <div class="ui pointing label">
                                  Please enter name of a show
                         </div>  
                         <br />
                         <br />
                         <div class="ui action input">
                              <input type="text" id="show_director" runat="server" />
                              <button class="ui blue right labeled icon button" OnServerClick="show_director_btn_OnClick" id="show_director_btn" runat="server" >
                                <i class="record icon"></i>
                                Set
                              </button>
                         </div>
                         <br />
                         <div class="ui pointing label">
                                  Please enter name of show's director
                         </div> 
                         <br />
                         <br />
                         <div class="ui action input">
                              <input type="text" id="show_goldenglobe" runat="server"  />
                              <button class="ui blue right labeled icon button" OnServerClick="show_goldenglobe_btn_OnClick" id="show_goldenglobe_btn" runat="server">
                                <i class="trophy icon"></i>
                                Set
                              </button>
                         </div>
                         <br />
                         <div class="ui pointing label">
                                  Golden Globe? (True/False)
                         </div>   
                         <br />
                         <br />
                         <div class="ui action input">
                              <input type="text" id="show_genre" runat="server"  />
                              <button class="ui blue right labeled icon button" OnServerClick="show_genre_btn_OnClick" id="show_genre_btn" runat="server">
                                <i class="tag icon"></i>
                                Set
                              </button>
                         </div>
                         <br />
                         <div class="ui pointing label">
                                  Please enter genre
                         </div> 
                         <br />
                         <br /> 
                         <button class="positive ui button" OnServerClick="insert_btn_OnClick" id="insert_btn" runat="server">    Insert Show    </button>
                     </div>
                </div>
            </div>
    </div>

</asp:Content>

