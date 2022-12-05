<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="client_chooseOrderPage.aspx.cs" Inherits="Lunch.client_page.client_chooseOrderPage" %>

<!DOCTYPE html>

<html>
<head>
    <title>Choose Order</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center;">
        <h1>今日餐廳: </h1>
        <h1 id="todayres"></h1>
        <h2>想吃什麼呢?</h2>
    </div>
    </form>
  
    
    
  <script>
      function choose() {
          
          var values = [];
          var h1 = document.getElementsByName("order");
          var h8 = document.getElementById('choo');
     

          h8.textContent = null;
           for(var i=0;i<h1.length;i++)
           {
               
               if (h1[i].checked == true) {
                   
                   values.push(h1[i].value);
                }
                               
           }
           h8.textContent = values.join(',');
         
        }
  </script>
    
    
    
    <form style="position:absolute;left:900px;" onsubmit="">
        
            <input type="checkbox" value="雞腿飯" name="order" onclick="choose()" id="chi"/>雞腿飯<BR>
            <input type="checkbox" value="炒飯" name="order" onclick="choose()" id="xrice"/>炒飯<BR>
            <input type="checkbox" value="牛肉麵" name="order" onclick="choose()" id="xnoo"/>牛肉麵<BR>
            <input type="checkbox" value="滷肉飯" name="order" onclick="choose()" id="rice"/>滷肉飯<BR>
            <input type="checkbox" value="湯" name="order" onclick="choose()" id="xx"/>湯<BR>
            <input type="checkbox" value="魚" name="order" onclick="choose()" id="xxx"/>魚<BR>
            <input type="checkbox" value="小菜" name="order" onclick="choose()" id="veg"/>小菜<BR><BR>
            <h3>你點了:</h3>
    <h3 id="choo"></h3>
            <input type="submit" />
            
                    
    </form>        
</body>
</html>
