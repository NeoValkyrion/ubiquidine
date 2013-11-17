<%@ Title="Ubiquidine" Page Async="true" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="WebApplication2._Default"  %>

<asp:Content ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent" runat="server">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>motha fuckin table tracker</h2>
    </hgroup>
    
    <span>Table ID: </span><asp:TextBox id="TextBox1"  runat="server" Width="20px"></asp:TextBox>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:ImageField HeaderText="thecup" DataImageUrlField="Images/empty_cup.jpg"></asp:ImageField>
            <asp:ImageField HeaderText="theplate" DataImageUrlField="Images/plate.jpg"></asp:ImageField>   
            <asp:ImageField HeaderText="theexclam" DataImageUrlField="Images/exclamation.jpg"></asp:ImageField>
        </Columns>
    </asp:GridView>
    <asp:Button text="dummy refresh button" id="Button1" runat="server" OnClick="thebutton_Click"/>

    <asp:GridView ID="GridView2" runat="server" BackColor="White" BorderColor="#999999"
        BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="100%">
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
</asp:Content>
