<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Device_Operations.aspx.cs" Inherits="WoT_Test_Environment.Device_Operations" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style3
        {
            color: #FF9933;
            text-align: left;
            font-size: medium;
            background-color: #000000;
            height: 472px;
            width: 938px;
            margin-top: 0px;
        }
        .menu
        {
            font-size: medium;
            margin-left: 23px;
            margin-right: 7px;
            margin-bottom: 1px;
            text-align: left;
        }
        .style4
        {
            color: #FF9933;
            text-align: left;
            font-size: medium;
            background-color: #000000;
            width: 954px;
            height: 67px;
            margin-top: 76px;
        }
        .style5
        {
            text-align: left;
        }
        .style6
        {
        }
        .style12
        {
            text-align: left;
            width: 10%;
        }
        .style13
        {
            width: 44%;
        }
        .style14
        {
            width: 10%;
            text-align: left;
            height: 20px;
        }
        .style15
        {
            height: 20px;
            text-align: left;
            width: 44%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p class="style3">
        <table style="border: thin solid #FFFF66; width: 100%; table-layout: auto; height: 470px;" 
            frame="box">
            <tr>
                <td class="style12">
                    &nbsp;&nbsp; Device Information&nbsp;</td>
                <td class="style13">
                    &nbsp;</td>
                <td class="style13">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style12">
&nbsp;&nbsp;&nbsp;&nbsp; Current State</td>
                <td class="style13">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lb_Cur_State" runat="server" 
                style="font-size: large; text-align: center; color: #FFFF00" Text=" Wait ..."></asp:Label>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:Timer ID="Timer1" runat="server" Interval="500" ontick="Timer1_Tick">
            </asp:Timer>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" />
        </Triggers>
    </asp:UpdatePanel>
    
                </td>
                <td class="style13">
                    Chart for Monitory Relation Between Current Reading and
                </td>
            </tr>
            <tr>
                <td class="style12">
                    &nbsp;&nbsp; All Service&#39;s URL&nbsp;</td>
                <td class="style6" colspan="2">
    
    <asp:BulletedList ID="BulletedList1" runat="server" CssClass="menu" 
     BackColor="#333333" BorderColor="#CCCCCC" 
        BorderStyle="Solid" BulletStyle="UpperRoman" Width="751px" ForeColor="#3399FF">
    </asp:BulletedList>
                </td>
            </tr>
            <tr>
                <td class="style5" colspan="2">
                    Select One Method from the following Dropdown List</td>
                <td class="style13">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style14">
                </td>
                <td class="style15">
        <asp:DropDownList ID="DropDownList1" runat="server" 
          
            style="text-align: left; margin-left: 0px;" Height="21px" Width="209px">
        </asp:DropDownList>
                </td>
                <td class="style15">
                </td>
            </tr>
            <tr>
                <td class="style14">
                    &nbsp;</td>
                <td class="style15">
 
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
        <ContentTemplate>
<asp:DropDownList ID="DropDownList2" runat="server" 
          
            style="text-align: left; margin-left: 1px;" Height="20px" Width="208px">
        </asp:DropDownList>
            <asp:Button ID="Bt_Parameters" runat="server" Height="30px" 
                onclick="Bt_Parameters_Click" style="text-align: left; margin-left: 25px;" 
                Text="Get Parameter List" Width="128px" />
        </ContentTemplate>
    </asp:UpdatePanel>
                </td>
                <td class="style15">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style14">
                    &nbsp;</td>
                <td class="style15">
        
        <asp:TextBox ID="TextBox2" runat="server" Height="20px" 
            style="margin-left: 0px; text-align: left;" Width="209px"></asp:TextBox>
        <asp:Button ID="bt_Param_Value" runat="server" onclick="bt_Param_Value_Click" 
            style="margin-left: 26px;" Text="Post Value" 
            Height="30px" Width="124px" />
                </td>
                <td class="style15">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
   <ContentTemplate>
 
        <asp:ListBox ID="ListBox1" runat="server" Height="50px" Width="248px" 
            style="margin-left: 35px">
        </asp:ListBox>
        <asp:Button ID="bt_Delete" runat="server" onclick="bt_Delete_Click" 
            style="text-align: left; margin-left: 17px;" Text="Delete Selected" 
            Height="30px" Width="100px" />
        </ContentTemplate>
        <Triggers>

            <asp:AsyncPostBackTrigger ControlID="bt_Param_Value" />

        </Triggers>
   </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="style14">
        <asp:Label ID="Label2" runat="server" Text="Execution Result"></asp:Label>
                </td>
                <td class="style15">
        <asp:TextBox ID="TextBox3" runat="server" Height="20px" 
            style="text-align: right; margin-left: 0px" Width="211px"></asp:TextBox>
        <asp:Button ID="bt_invoke" runat="server" onclick="bt_Invoke_Click" 
            style="text-align: left; margin-left: 25px;" Text="Invoke Method" 
            Height="30px" Width="123px" />
                </td>
                <td class="style15">
                    &nbsp;</td>
            </tr>
        </table>
    </p>

 </asp:Content>
