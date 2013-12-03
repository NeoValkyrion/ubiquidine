<%@ Page Title="Ubiquidine" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>



<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">



<link href="http://fonts.googleapis.com/css?family=Dancing+Script" rel="stylesheet" type="text/css">
<link rel="stylesheet" href="http://www.zishanbudhwani.com/main3.css">
<link rel="stylesheet" href="http://www.zishanbudhwani.com/ubiquicss.css">
 
<script>
    $(document).ready(function () {
     setTimeout(CheckStatus, 3000);
 });

    function CheckStatus() {
        var options = {};
        options.url = "Default.aspx/GetStatus";
        options.type = "POST";
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (data) {
            var i;
            for (i in data) {
                if (data.hasOwnProperty(i)) {
                    console.log(data); 
                    console.log("" + i + "    " + data[i]); 
                }
                if (data.d.charAt(0) == "1") {
                    $("#FeaturedContent_theexclam").css('visibility', 'visible');
                }
                else {
                    //alert("" + data.d);
                    $("#FeaturedContent_theexclam").css('visibility', 'hidden');
                }
                if (data.d.charAt(1) == "1") {
                    $("#FeaturedContent_theplate").css('visibility', 'visible');
                }
                else {
                    //alert("" + data.d);
                    $("#FeaturedContent_theplate").css('visibility', 'hidden');
                }
                if (data.d.charAt(2) == "1") {
                    $("#FeaturedContent_thecup").css('visibility', 'visible');
                }
                else {
                    //alert("" + data.d);
                    $("#FeaturedContent_thecup").css('visibility', 'hidden');
                }
            }
            //if (!data.d) {
                setTimeout(CheckStatus, 2000);
            //}
        };
        $.ajax(options);
    }


</script>


<body class="body_posts body_channel" data-jsid="posts/channel" data-posts="tech" data-sec0="tech" data-sec1="index" viewport="fixed" style="">
<header id="site-header"><div class="navbar">
<div class="navbar-inner"><ul class="main-menu nav inline">
<li class="menu"><a class="icon-reorder" href="http://www.zishanbudhwani.com/ubiquidine.html"></a></li>
</ul></div>
</div></header>

<div style=" background:url('http://www.zishanbudhwani.com/fotree.jpg'); color:black; margin-top: -2.5%; padding-bottom: 10%;">
<p style="margin-left: 45%; font-size:38px; font-family: 'Dancing Script', arial, sans-serif; color:black;"></p>
<div id="container">   

	<table class="zebra">
    <caption style="font-size:32px; font-family: 'Dancing Script', arial, sans-serif; color:white;">Ubiquidine - Waiter View</caption>
		<thead>
        	<tr>
				<th>Table ID</th>
		
				<th>Status</th>
            </tr>
		</thead>
        <tbody>
        	<tr>
            	<td>01</td>

                <td><img id="thecup" src="Images/Cup.png" height="50" runat="server" style="visibility:hidden" />
                 <img id="theplate" src="Images/Plate.png" height="50" runat="server" style="visibility:hidden" />
                 <img id="theexclam" src="Images/Waiter.png" height="50" runat="server" style="visibility:hidden" />
                 <asp:Button ID="resetButton" Text="Clear" OnClick="clearTable" runat="server" />
                </td>
            </tr>
        </tbody>
	</table>
</div>
</div>



<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>




</body></html>

</asp:Content>
