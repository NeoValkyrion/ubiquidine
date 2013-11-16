<%@ Page Title="Ubiquidine" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>



<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">


            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>motha fuckin table tracker</h2>
            </hgroup>
            <p>
                <span>Table ID: </span><asp:TextBox id="TextBox1"  runat="server" Width="20px"></asp:TextBox>
                 <img id="thecup" src="Images/empty_cup.jpg" height="50" runat="server" visible="false" />
                 <img id="theplate" src="Images/plate.jpg" height="50" runat="server" visible="false" />
                 <img id="theexclam" src="Images/exclamation.jpg" height="50" runat="server" visible="false" />
                 <asp:Button text="dummy refresh button" id="thebutton" runat="server" OnClick="thebutton_Click"/>
            </p>


</asp:Content>
