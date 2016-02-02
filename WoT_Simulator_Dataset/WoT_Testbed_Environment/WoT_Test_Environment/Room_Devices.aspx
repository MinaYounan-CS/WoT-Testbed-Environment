<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Room_Devices.aspx.cs" Inherits="WoT_Test_Environment.Room_Devices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style2
        {
            color: #66FF33;
        }
        .style3
        {
            text-align: center;
            font-size: large;
        }
        .style5
        {
            text-align: center;
            font-size: large;
            clear: both;
            color: #FFFFFF;
            background-color: #333333;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p class="style5" 
        
        
        style="border-left: thin none #CCCCCC; border-right: thin none #CCCCCC; border-top: thin none #CCCCCC; border-bottom: thin solid #CCCCCC; width: 923px; margin-left: 11px;">
        Select One Device from the following Table: &nbsp;</p>
    <p style="background-color: #000000" align="center">
        <asp:GridView ID="GridView1" runat="server" BackColor="#FFFF99" 
            BorderColor="#FFFF66" BorderStyle="Solid" BorderWidth="2px" CellPadding="3" 
            CellSpacing="1" style="margin-left: 6px; margin-top: 23px;" Width="928px" 
            AutoGenerateColumns="False" 
            ShowHeaderWhenEmpty="True" Font-Names="Arial" ForeColor="Black">
            

           
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
        <FooterStyle BackColor="#FF66FF" BorderColor="#FFCCFF" BorderStyle="Solid" />
        <HeaderStyle BackColor="#FF9999" Font-Bold="True" BorderColor="Black" 
            BorderStyle="Inset" ForeColor="Black" HorizontalAlign="Center" 
                VerticalAlign="Middle" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
            HorizontalAlign="Center" />
            <RowStyle BorderColor="Black" BorderStyle="Solid" HorizontalAlign="Center" 
                VerticalAlign="Middle" />
        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <SortedAscendingCellStyle BackColor="#FAFAE7" />
        <SortedAscendingHeaderStyle BackColor="#DAC09E" />
        <SortedDescendingCellStyle BackColor="#E1DB9C" />
        <SortedDescendingHeaderStyle BackColor="#C2A47B" />
            
         <Columns>
          <asp:HyperLinkField DataTextField="title" DataNavigateUrlFields="id" 
                 DataNavigateUrlFormatString="~/Device_Services.aspx?Id={0}" 
                 HeaderText="Title" ItemStyle-Width = "150" >
             <ControlStyle Width="100px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="150px"></ItemStyle>
             </asp:HyperLinkField>
          <asp:BoundField DataField="building_id" HeaderText="Building_Id" 
                 ItemStyle-Width = "50" >
             <ControlStyle Width="20px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="50px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="type" HeaderText="Type" ItemStyle-Width = "100" >
             <ControlStyle Width="20px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="100px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="pin_id" HeaderText="Pin_Id" ItemStyle-Width = "50" >
             <ControlStyle Width="20px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="50px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="gateway_id" HeaderText="Gateway_Id" 
                 ItemStyle-Width = "50" >
             <ControlStyle Width="20px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="50px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="serial" HeaderText="Serial" ItemStyle-Width = "100" >
             <ControlStyle Width="20px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="100px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="model" HeaderText="Model" ItemStyle-Width = "100" >
             <ControlStyle Width="20px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="100px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="manufacturer" HeaderText="Manufacturer" 
                 ItemStyle-Width = "100" >
             <ControlStyle Width="50px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="100px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="description" HeaderText="Description" 
                 ItemStyle-Width = "250" >
             <ControlStyle Width="200px" />
             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />
<ItemStyle Width="250px"></ItemStyle>
             </asp:BoundField>
          <asp:BoundField DataField="ws_address" HeaderText="Address(WebServices)" 
                 ItemStyle-Width = "200" >

             <ControlStyle Width="200px" />

             <HeaderStyle BackColor="#CC3300" Font-Bold="True" Font-Names="Georgia" />

<ItemStyle Width="200px"></ItemStyle>
             </asp:BoundField>

        </Columns>

        </asp:GridView>
    </p>
    </asp:Content>
