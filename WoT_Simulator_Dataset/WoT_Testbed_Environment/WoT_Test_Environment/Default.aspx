<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WoT_Test_Environment._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .style7
    {
        color: #990000;
    }
        .style8
        {
            color: #FFFF00;
            text-align: center;
            background-color: #333333;
        }
        .style12
        {
            color: #FF9900;
            font-family: Andalus;
            font-size: large;
        }
        .style13
        {
            text-align: center;
        }
        .style14
        {
            color: #FF9900;
            text-align: justify;
        }
        .style15
        {
            color: #FFFFFF;
        }
        .style16
        {
            font-size: 1.2em;
        }
        .style17
        {
            font-size: 0.8em;
        }
        .style18
        {
            color: #FF3300;
            font-weight: normal;
            font-family: Consolas;
            font-size: medium;
        }
        </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2 class="style8">
    Welcome to Web of things Testbed (WoTT) !&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
        <span class="style18">Offline Mode</span><span class="style17"> </span>
</h2>
<p class="style14" 
        
    
    
    style="border-left: thin none #CCCCCC; border-right: thin none #CCCCCC; border-top: thin inset #CCCCCC; border-bottom: thin inset #CCCCCC; background-color: #000000; width: 915px; margin-left: 13px; border-style: inset;">
    &nbsp;&nbsp;&nbsp;&nbsp; <span class="style16">
    <br />
&nbsp;&nbsp;&nbsp; Using this Testbed, You can work with the WoT Online 
    and Offline By selecting which mehod that you want, where webservices can fetch 
    information about device from the network (IoT) or from Dataset files (previously, generated 
    by the testbed). rules on which this testbed was built follows the 
    architecture proposed in the paper <span class="style15">[M. Younan; S. 
    Khattab; R. Bahgat, &quot; An Integerated Testbed Environment for the Web of Things&quot;, 
    The Eleventh International Conference on Networking and Services-ICNS2015, 
    IARIA, Rome-Italy, 2015]</span>. Smart Home include number of floors; each 
    floor includes number of rooms; each room housts number of device; each device 
    suports number of services that can be executed on it (RESTful Services).<br />
    <br />
    </span></p>
<p class="style3" style="background-color: #000000" align="center">
    <asp:Image ID="Image1" runat="server" Height="197px" 
        ImageUrl="~/Pictures/House.png" Width="227px" />
    <asp:Image ID="Image4" runat="server" Height="154px" 
        ImageUrl="~/Pictures/next.png" Width="64px" />
    <asp:Image ID="Image2" runat="server" Height="194px" 
        ImageUrl="~/Pictures/floor.png" Width="253px" style="text-align: center" />
    <asp:Image ID="Image5" runat="server" Height="145px" 
        ImageUrl="~/Pictures/next.png" Width="64px" />
    <asp:Image ID="Image3" runat="server" Height="192px" 
        ImageUrl="~/Pictures/room.png" Width="255px" style="text-align: center" />
</p>
<p class="style13" 
    
    style="border-left: thin none #CCCCCC; border-right: thin none #CCCCCC; border-top: thin inset #CCCCCC; border-bottom: thin inset #CCCCCC; border-style: inset none none none; color: #FF0000;">
    <span class="style12">
    <br />
    Select Room ID to Load Devices you want to monitor 
    or to control them</span><strong class="style7"> </strong>


</p>
<p style="background-color: #000000">

    <asp:GridView ID="GridView1" runat="server" 
        BackColor="#999999" BorderColor="#999966" BorderStyle="Solid" BorderWidth="3px" 
        CellPadding="2" ForeColor="Black" Width="921px" Height="126px" 
        HorizontalAlign="Center" onrowdatabound="GridView1_RowDataBound" 
        onselectedindexchanged="GridView1_SelectedIndexChanged" 
        AutoGenerateColumns="False">
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <FooterStyle BackColor="Tan" />
        <HeaderStyle BackColor="#FF9999" Font-Bold="True" BorderColor="Black" 
            BorderStyle="Inset" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
            HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <SortedAscendingCellStyle BackColor="#FAFAE7" />
        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
        <SortedDescendingCellStyle BackColor="#E1DB9C" />
        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
         <Columns>
        <asp:HyperLinkField DataTextField="id" DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/Room_Devices.aspx?Id={0}"
            HeaderText="id" ItemStyle-Width = "50" >
<ItemStyle Width="50px" BorderColor="White" BorderStyle="Solid" HorizontalAlign="Center" 
                 VerticalAlign="Middle"></ItemStyle>
             </asp:HyperLinkField>
              <asp:BoundField DataField="title" HeaderText="title" ItemStyle-Width = "100" >
<ItemStyle Width="100px" BorderColor="White" BorderStyle="Solid" HorizontalAlign="Center" 
                 VerticalAlign="Middle"></ItemStyle>
             </asp:BoundField>
        <asp:BoundField DataField="root_id" HeaderText="Floor_ID" ItemStyle-Width = "100" >
<ItemStyle Width="100px" BorderColor="White" BorderStyle="Solid" HorizontalAlign="Center" 
                 VerticalAlign="Middle"></ItemStyle>
             </asp:BoundField>
        <asp:BoundField DataField="type" HeaderText="type" ItemStyle-Width = "100" >
<ItemStyle Width="100px" BorderColor="White" BorderStyle="Solid" HorizontalAlign="Center" 
                 VerticalAlign="Middle"></ItemStyle>
             </asp:BoundField>
        <asp:BoundField DataField="description" HeaderText="Description" 
                 ItemStyle-Width = "250" >
<ItemStyle Width="250px" BorderColor="White" BorderStyle="Solid" HorizontalAlign="Center" 
                 VerticalAlign="Middle"></ItemStyle>
             </asp:BoundField>
    </Columns>
    </asp:GridView>
</p>
<p class="style2" style="background-color: #000000; color: #FF0000;" align="center">
    
</p>
</asp:Content>
