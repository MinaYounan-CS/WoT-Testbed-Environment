<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Device_Services.aspx.cs" Inherits="WoT_Test_Environment.Device_Services" %>
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .menu
        {
            font-size: medium;
            margin-left: 13px;
            margin-right: 7px;
            margin-bottom: 1px;
            }
        .style10
        {
            width: 1%;
            height: 54px;
        }
        .style15
        {
            width: 116px;
            background-color: #999999;
        }
        .style16
        {
            width: 116px;
            height: 54px;
            background-color: #666666;
        }
        .style18
        {
            height: 54px;
            width: 393px;
            background-color: #666666;
        }
        .style20
        {
            width: 1%;
        }
        .style29
        {
            width: 1%;
            height: 35px;
        }
        .style30
        {
            height: 35px;
            font-size: large;
            color: #FFFF66;
            background-color: #666666;
            width: 116px;
        }
        .style31
        {
            height: 35px;
            width: 393px;
            background-color: #666666;
        }
        .style32
        {
            width: 27%;
            height: 35px;
            color: #FFFF00;
            font-size: medium;
            text-align: center;
            background-color: #9966FF;
        }
        .style33
        {
            width: 48%;
            background-color: #333333;
        }
        .style35
        {
            background-color: #333333;
        }
        .style36
        {
            background-color: #333333;
            text-align: center;
        }
        .style37
        {
            width: 1%;
            height: 17px;
        }
        .style38
        {
            height: 17px;
            font-size: large;
            color: #FFFF66;
            background-color: #333333;
        }
        .style42
        {
            width: 27%;
        }
        .style44
        {
            width: 393px;
            background-color: #999999;
        }
        .style45
        {
            background-color: #333333;
            width: 237px;
        }
        .style47
        {
            width: 1%;
            height: 39px;
        }
        .style48
        {
            width: 116px;
            height: 39px;
            background-color: #999999;
        }
        .style49
        {
            width: 393px;
            height: 39px;
            background-color: #999999;
        }
        .style50
        {
            background-color: #000000;
            text-align: center;
            font-size: medium;
            color: #FFCC66;
            font-family: "Arial Unicode MS";
        }
        .style52
        {
            text-align: center;
            font-size: large;
        }
        .style53
        {
            background-color: #333333;
            width: 116px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="border: thin solid #FF9933; width: 100%;">
        <tr>
            <td class="style50" colspan="5">
                <div class="style52">
                Device Details<br />
                    <br />
                </div>
                <asp:GridView ID="GridView1" runat="server" AllowSorting="True" Height="16px" 
                    HorizontalAlign="Center" Width="887px" 
                   
                    style="text-align: justify" BorderColor="Black" BorderStyle="Solid" 
                    Font-Bold="False" Font-Size="Medium">
                    <HeaderStyle BorderColor="#999966" BorderStyle="Solid" HorizontalAlign="Center" 
                        VerticalAlign="Middle" BackColor="Black" Font-Bold="False" 
                        Font-Names="Arial" Font-Overline="False" Font-Strikeout="False" 
                        Font-Underline="False" ForeColor="#FF9933" />
                    <RowStyle BorderColor="#999966" BorderStyle="Solid" 
                        HorizontalAlign="Center" VerticalAlign="Middle" Font-Names="Arial" 
                        Font-Size="Medium" ForeColor="#FFCC66" />
                </asp:GridView>
                <br />
            </td>
        </tr>
        <tr>
            <td class="style36">
                &nbsp;</td>
            <td class="style53">
                &nbsp;</td>
            <td class="style45">
                &nbsp;</td>
            <td class="style33" colspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="5" style="text-align: center">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lb_Cur_State" runat="server" 
                style="font-size: large; text-align: center; color: #FFFF00" Text=" Wait ..."></asp:Label>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:Chart ID="chtCategoriesProductCount" runat="server" 
                BackSecondaryColor="255, 128, 128" BorderlineColor="Black" 
                BorderlineDashStyle="Solid" EnableViewState="True" Height="200px" 
                Palette="EarthTones" style="text-align: center; margin-left: 0px" Width="884px">
                <Series>
                    <asp:Series ChartArea="MainChartArea" ChartType="Spline" Name="Categories" 
                        Palette="Chocolate">
                    </asp:Series>
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="MainChartArea">
                    </asp:ChartArea>
                </ChartAreas>
                <BorderSkin BackColor="Black" BackGradientStyle="HorizontalCenter" 
                    BackHatchStyle="DiagonalCross" BackSecondaryColor="255, 224, 192" 
                    BorderColor="Silver" BorderDashStyle="Solid" PageColor="Black" 
                    SkinStyle="FrameThin5" />
            </asp:Chart>
            <asp:Timer ID="Timer1" runat="server" Interval="1000" ontick="Timer1_Tick">
            </asp:Timer>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" />
        </Triggers>
    </asp:UpdatePanel>
    
                </td>
        </tr>
        <tr>
            <td colspan="5" style="text-align: left">
                &nbsp;</td>
        </tr>
        <tr>
            <td class="style37">
            </td>
            <td class="style38" colspan="4">
                Select Device Service from the following:</td>
        </tr>
        <tr>
            <td class="style29">
            </td>
            <td bgcolor="#FFCCCC" class="style30">
        <asp:DropDownList ID="DropDownList1" runat="server" 
          
            style="text-align: left; " Height="22px" Width="209px">
        </asp:DropDownList>
                </td>
            <td bgcolor="#FFCCCC" class="style31" colspan="2">
 
        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
<asp:DropDownList ID="DropDownList2" runat="server" 
          
            style="text-align: left; margin-left: 1px;" Height="22px" Width="147px">
        </asp:DropDownList>
            <asp:Button ID="Bt_Parameters" runat="server" Height="30px" 
                onclick="Bt_Parameters_Click" style="text-align: left; margin-left: 25px;" 
                Text="Parameters" Width="80px" />
        </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Bt_Parameters" />
            </Triggers>
    </asp:UpdatePanel>
                </td>
            <td class="style32">
                &nbsp;&nbsp;&nbsp; Dynamic Links For Services&nbsp;</td>
        </tr>
        <tr>
            <td class="style20">
                &nbsp;</td>
            <td bgcolor="#FFCCCC" class="style15">
        
        <asp:TextBox ID="TextBox2" runat="server" Height="20px" 
            style="margin-left: 0px; text-align: left;" Width="209px"></asp:TextBox>
            </td>
            <td bgcolor="#FFCCCC" class="style44" colspan="2">
        <asp:Button ID="bt_Param_Value" runat="server" onclick="bt_Param_Value_Click" 
            style="margin-left: 0px;" Text="Post" 
            Height="30px" Width="100px" />
                </td>
            <td bgcolor="#FFCC99" class="style42" rowspan="3">
    
    <asp:BulletedList ID="BulletedList1" runat="server" CssClass="menu" 
     BackColor="#CCCCCC" BorderColor="White" 
        BorderStyle="Groove" BulletStyle="Disc" Width="208px" ForeColor="#0000CC" 
                    Height="110px">
    </asp:BulletedList>
                </td>
        </tr>
        <tr>
            <td class="style10">
            </td>
            <td bgcolor="#FFCCCC" class="style16">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
   <ContentTemplate>
 
        <asp:ListBox ID="ListBox1" runat="server" Height="80px" Width="285px" 
            style="margin-left: 1px" Rows="5">
        </asp:ListBox>
        </ContentTemplate>
        <Triggers>

            <asp:AsyncPostBackTrigger ControlID="bt_Param_Value" />

            <asp:AsyncPostBackTrigger ControlID="bt_Delete" />

        </Triggers>
   </asp:UpdatePanel>
                </td>
            <td bgcolor="#FFCCCC" class="style18" colspan="2">
        <asp:Button ID="bt_Delete" runat="server" onclick="bt_Delete_Click" 
            style="text-align: left; margin-left: 1px;" Text="Delete" 
            Height="30px" Width="99px" />
            </td>
        </tr>
        <tr>
            <td class="style47">
            </td>
            <td bgcolor="#FFCCCC" class="style48">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="TextBox3" runat="server" Height="20px" 
            style="text-align: center; margin-left: 1px" Width="276px"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="bt_invoke" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td bgcolor="#FFCCCC" class="style49" colspan="2">
        <asp:Button ID="bt_invoke" runat="server" onclick="bt_Invoke_Click" 
            style="text-align: left; margin-left: 0px;" Text="Invoke" 
            Height="30px" Width="99px" />
                </td>
        </tr>
        <tr>
            <td bgcolor="#333333" class="style20">
                &nbsp;</td>
            <td class="style35" colspan="4">
                &nbsp;</td>
        </tr>
    </table>
</asp:Content>
