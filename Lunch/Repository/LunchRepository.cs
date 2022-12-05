using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Lunch.Entity;
using Dapper;

namespace Lunch.Repository
{
    public class LunchRepository
    {
        private int DefaultRestaurantId = 54;

        public readonly SqlConnection _Connection = new SqlConnection(ConfigurationManager.AppSettings["dbconfig"]);

        public IEnumerable<Restaurant> GetRestaurants() //
        {
            return _Connection.Query<Restaurant>("[dbo].[GetAllRestaurants]", CommandType.StoredProcedure);
        }

        public List<Restaurant> GetPushedRestaurant() //
        {
            var restaurants = _Connection.Query<Restaurant>("[dbo].[GetPushedRestaurant]", CommandType.StoredProcedure).ToList();
            return restaurants.Any() ? restaurants : new List<Restaurant>() { GetRestaurantById(DefaultRestaurantId) };
        }

        public IEnumerable<Customer> GetCustomerName() //public static 型別<model名稱> StoreProcedure名稱()
        {
            return _Connection.Query<Customer>("[dbo].[GetCustomerName]", CommandType.StoredProcedure);
            //透過GetCustomerName可以取得所有的customer的註冊名稱
        }

        public Customer GetLoginInformation(string name, string password)
        {
            var para = new DynamicParameters();
            para.Add("@name", name);
            para.Add("@password", password);

            return
                _Connection.Query<Customer>("[dbo].[LogInCheck]", para, null, true, null, CommandType.StoredProcedure)
                    .FirstOrDefault(); //FirstOrDefault() 取第一個資料或是NULL
        }

        public IEnumerable<OrderInfo> GetAllOrder(int rid) //public static model名稱 StoreProcedure名稱()
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid); //把Rid傳進SQL做為query的條件
            para.Add("@nowtime", DateTime.Now); //將C#中的時間用para参數傳到SQL中，讓SQL可以抓到現在本地的時間

            return _Connection.Query<OrderInfo>("[dbo].[GetAllOrder]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetAllOrder可以取得今天所有餐點
        }

        public IEnumerable<Customer> GetNotOrder(int rid) //public static model名稱 StoreProcedure名稱()        //
        {
            var para = new DynamicParameters();
            para.Add("@nowtime", DateTime.Now);
            para.Add("@rid", rid);
            var i = _Connection.Query<Customer>("[dbo].[GetNotOrder]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetNotOrder可以取得當天誰沒有點餐

            // _Connection.Dispose();
            return i;
        }

        public IEnumerable<IndCost> IndividualCost() //public static model名稱 StoreProcedure名稱()
        {
            var para = new DynamicParameters();
            para.Add("@nowtime", DateTime.Now);

            return _Connection.Query<IndCost>("[dbo].[IndividualCost]", para, null, true, null,
                CommandType.StoredProcedure); //過IndividualCost可以取得每個人當日點餐的金額加總
        }

        public IEnumerable<OrderInfo> GroupByMeal(int rid)
        {
            var para = new DynamicParameters();
            para.Add("@nowdate", DateTime.Now);
            para.Add("@Rid", rid);
            var i = _Connection.Query<OrderInfo>("[dbo].[GroupByMeal]", para, null, true, null,
                CommandType.StoredProcedure); //透過GroupByMeal取得當日點餐資訊
            // _Connection.Dispose();
            return i;
        }

        public IEnumerable<OrderInfo> GetTotalOrderInformation(int rid) //透過GetTotalOrderInformation取得當天選定的餐廳的點餐記錄
        {
            var para = new DynamicParameters();
            para.Add("@date", DateTime.Now);
            para.Add("@Rid", rid);
            var i = _Connection.Query<OrderInfo>("[dbo].[GetTotalOrderInformation]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetTotalOrderInformation取得當日點餐資訊
            //  _Connection.Dispose();
            return i;
        }

        public IEnumerable<Summary> GetBalance(DateTime date1, DateTime date2) //透過GetBalance取得兩日期間所有人的Balance
        {
            var para = new DynamicParameters();
            para.Add("@date1", date1);
            para.Add("@date2", date2);
            return _Connection.Query<Summary>("[dbo].[GetBalance]", para, null, true, null, CommandType.StoredProcedure);
        }

        public IEnumerable<Summary> GetIndBalance(DateTime date1, DateTime date2, int Cid)
        //透過GetIndBalance取得兩日期間特定人的Balance
        {
            var para = new DynamicParameters();
            para.Add("@date1", date1);
            para.Add("@date2", date2);
            para.Add("@Cid", Cid);
            return _Connection.Query<Summary>("[dbo].[GetIndBalance]", para, null, true, null,
                CommandType.StoredProcedure);
        }

        public IEnumerable<Customer> GetRichestUser()
        {
            var result = _Connection.Query<Customer>(@"select top 1 [Summary].Name,[Balance]
	from [Summary]
	join (select [Name], max([SummaryId]) as maxId 
		  from [Summary] 
		  group by [Name]) as temp on [SummaryId] in (temp.maxId)
    RIGHT JOIN(
		SELECT [Name]
		FROM [Customer] 
		WHERE [Permission]<>'4'
	)p
	on p.[Name]  =[Summary].[Name] 
	order by Balance desc").ToList();
            return result;
        }

        public Restaurant SelectRid() //透過SelectRid拿取所有的Rid，但是表示settled的圖片不拿
        {
            return _Connection.Query<Restaurant>("[dbo].[SelectRid]", CommandType.StoredProcedure).FirstOrDefault();
        }

        public void InsertRestaurant(string name, string tel, string url, int category)
        {
            var para = new DynamicParameters();
            para.Add("@name", name);
            para.Add("@tel", tel);
            para.Add("@url", url);
            para.Add("@cate", category);
            para.Add("@modtime", DateTime.Now);
            _Connection.Execute("[dbo].[InsertNewRestaurant]", para, null, null, CommandType.StoredProcedure);
            //return true;
        }

        public void UpdateRestaurant(string name, string tel, string url, string comment, int rid, int category)
        {
            var para = new DynamicParameters();
            para.Add("@name", name);
            para.Add("@tel", tel);
            para.Add("@url", url);
            para.Add("@cate", category);
            para.Add("@modtime", DateTime.Now);
            para.Add("@comment", comment);
            para.Add("@rid", rid);
            _Connection.Execute("[dbo].[UpdateRestaurant]", para, null, null, CommandType.StoredProcedure);
            //return true;
        }

        public IEnumerable<Customer> GetCustomerBalance(int limit) //透過GetCustomerBalance取得帳戶金額在limit金額值以下的名單
        {
            var para = new DynamicParameters();
            para.Add("@limit", limit);
            return _Connection.Query<Customer>("[dbo].[GetCustomerBalance]", para, null, true, null,
                CommandType.StoredProcedure);
        }

        public Restaurant GetRestaurantById(int Rid)
        {
            var para = new DynamicParameters();
            para.Add("@Rid", Rid);
            var restaurant =
                _Connection.Query<Restaurant>("[dbo].[GetRestaurantById]", para, null, true, null,
                    CommandType.StoredProcedure).FirstOrDefault();
            return restaurant;
        }

        public void DeleteRestaurantById(int Rid)
        {
            var para = new DynamicParameters();
            para.Add("@Rid", Rid);
            _Connection.Execute("[dbo].[DeleteRestaurantById]", para, null, null, CommandType.StoredProcedure);
        }

        public void DeleteOrder(int Oid)
        {
            var para = new DynamicParameters();
            para.Add("@Oid", Oid);
            _Connection.Execute("[dbo].[DeleteOrder]", para, null, null, CommandType.StoredProcedure);
        }

        public IEnumerable<Customer> GetName() //public static 型別<model名稱> StoreProcedure名稱()
        {
            var para = new DynamicParameters();
            para.Add("@month", 3);
            return _Connection.Query<Customer>("[dbo].[GetActiveCustomerList_161201]", para, null, true, null, CommandType.StoredProcedure);
            //透過GetName可以取得所有的customer的註冊名稱
        }

        public void AddRegistDataToSummary(string name, int cid)
        {
            var para = new DynamicParameters();
            para.Add("@name", name);
            para.Add("@cid", cid);
            para.Add("@time", DateTime.Now);
            _Connection.Execute("[dbo].[AddRegistDataToSummary]", para, null, null, CommandType.StoredProcedure);
            //return true;
        }

        public void UpdatePermission(int permission, int cid)
        {
            var para = new DynamicParameters();
            para.Add("@per", permission);
            para.Add("@cid", cid);
            _Connection.Execute("[dbo].[UpdatePermission]", para, null, null, CommandType.StoredProcedure);
            //return true;
        }

        public void UpdateAdmin(int cid, bool admin)
        {
            var para = new DynamicParameters();
            para.Add("@cid", cid);
            para.Add("@admin", admin);
            _Connection.Execute("[dbo].[UpdateAdminLevel]", para, null, null, CommandType.StoredProcedure);
            //return true;
        }

        public void DeleteCustomer(int cid)
        {
            var para = new DynamicParameters();
            para.Add("@Cid", cid);
            _Connection.Execute("[dbo].[DeleteCustomer]", para, null, null, CommandType.StoredProcedure);
        }

        public OrderInfo GetLatestOrder()
        {
            var LatestOrder = _Connection.Query<OrderInfo>("[dbo].[GetLatestOrder]", CommandType.StoredProcedure).First();
            return LatestOrder;
        }

        public IEnumerable<Restaurant> GetNotSettledRes()
        {
            return _Connection.Query<Restaurant>("[dbo].[GetNotSettledRes]", CommandType.StoredProcedure);
        }

        public void UpdateSettleByRid(int rid) //選定餐廳中settle=false的更改為true
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid);
            _Connection.Execute("[dbo].[UpdateSettleByRid]", para, null, null, CommandType.StoredProcedure);
        }

        public Order OrderTime(int rid) //找出所選點餐時的時間
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid);
            var time =
                _Connection.Query<Order>("[dbo].[OrderTime]", para, null, true, null, CommandType.StoredProcedure)
                    .FirstOrDefault();
            return time;
        }

        public int GetCustomerIdByName(string Name) //找出所選點餐時的時間
        {
            var para = new DynamicParameters();
            para.Add("@name", Name);
            var CId =
                _Connection.Query<int>("[dbo].[GetCustomerIdByName]", para, null, true, null,
                    CommandType.StoredProcedure).FirstOrDefault();
            return CId;
        }

        public IEnumerable<int> GetNotSettledResById(int Rid)
        {
            var para = new DynamicParameters();
            para.Add("@Rid", Rid);
            return _Connection.Query<int>("[dbo].[GetNotSettledResByRid]", para, null, true, null,
                CommandType.StoredProcedure);
        }

        public string GetNameById(int cid) //找出所選點餐時的時間
        {
            var para = new DynamicParameters();
            para.Add("@Cid", cid);
            var name =
                _Connection.Query<string>("[dbo].[GetNameById]", para, null, true, null, CommandType.StoredProcedure)
                    .FirstOrDefault();
            return name;
        }

        public IEnumerable<Restaurant> GetRestaurantByCategory(int Cate)
        {
            var para = new DynamicParameters();
            para.Add("@cate", Cate); //把Rid傳進SQL做為query的條件
            return _Connection.Query<Restaurant>("[dbo].[GetRestaurantByCategory]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetAllOrder可以取得今天所有餐點
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------------


        public IEnumerable<OrderInfo> GetAllOrder2(int rid) //public static model名稱 StoreProcedure名稱()
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid); //把Rid傳進SQL做為query的條件

            return _Connection.Query<OrderInfo>("[dbo].[GetAllOrder2]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetAllOrder可以取得今天所有餐點
        }

        public IEnumerable<OrderInfo> GetTransferList() //public static model名稱 StoreProcedure名稱()
        {
            return _Connection.Query<OrderInfo>("[dbo].[GetTransferRequest]", null, null, true, null,
                CommandType.StoredProcedure); //透過GetAllOrder可以取得今天所有餐點
        }

        public IEnumerable<Customer> GetNotOrder2(int rid) //public static model名稱 StoreProcedure名稱()        //
        {
            var para = new DynamicParameters();
            para.Add("@rid", rid);
            var i = _Connection.Query<Customer>("[dbo].[GetNotOrder2]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetNotOrder可以取得當天誰沒有點餐

            // _Connection.Dispose();
            return i;
        }

        public IEnumerable<OrderInfo> GetTotalOrderInformation2(int rid) //透過GetTotalOrderInformation取得當天選定的餐廳的點餐記錄
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid);
            var i = _Connection.Query<OrderInfo>("[dbo].[GetTotalOrderInformation2]", para, null, true, null,
                CommandType.StoredProcedure); //透過GetTotalOrderInformation取得當日點餐資訊
            //  _Connection.Dispose();
            return i;
        }

        public IEnumerable<OrderInfo> GroupByMeal2(int rid)
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid);
            var i = _Connection.Query<OrderInfo>("[dbo].[GroupByMeal2]", para, null, true, null,
                CommandType.StoredProcedure); //透過GroupByMeal取得當日點餐資訊
            // _Connection.Dispose();
            return i;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<OrderInfo> CheckResSettle(int rid) //取得餐廳未settle的order
        {
            var para = new DynamicParameters();
            para.Add("@Rid", rid);
            var i = _Connection.Query<OrderInfo>("[dbo].[CheckResSettle]", para, null, true, null,
                CommandType.StoredProcedure);
            return i;
        }

        public IEnumerable<Customer> GetCustomerName2() //從Summary取得Name.Balance(無被刪除成員資料)
        {
            return _Connection.Query<Customer>("[dbo].[GetCustomerName2]", CommandType.StoredProcedure);
            //透過GetCustomerName可以取得所有的customer的註冊名稱
        }

        public IEnumerable<Summary> GetBalance2(DateTime date1, DateTime date2)
        //透過GetBalance2取得兩日期間所有人的Balance(無被刪除成員資料)
        {
            var para = new DynamicParameters();
            para.Add("@date1", date1);
            para.Add("@date2", date2);
            return _Connection.Query<Summary>("[dbo].[GetBalance2]", para, null, true, null, CommandType.StoredProcedure);
        }

        public IEnumerable<Customer> GetCustomerBalance2(int limit) //透過GetCustomerBalance取得帳戶金額在limit金額值以下的名單(無被刪除成員資料)
        {
            var para = new DynamicParameters();
            para.Add("@limit", limit);
            return _Connection.Query<Customer>("[dbo].[GetCustomerBalance2]", para, null, true, null,
                CommandType.StoredProcedure);
        }

        public IEnumerable<Customer> GetName2() //public static 型別<model名稱> StoreProcedure名稱()
        {
            return _Connection.Query<Customer>("[dbo].[GetNameFromCustomer2]", CommandType.StoredProcedure);
            //透過GetName2可以取得所有的customer的註冊名稱(無被刪除成員資料)
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int RecentBalance(string name)
        {
            return
                _Connection.Query<int>(
                    "SELECT [Balance] FROM [Summary] WHERE [Name]=@TU_name ORDER BY [SummaryId] desc",
                    new { TU_name = name }).FirstOrDefault();
        }

        public Restaurant GetRestaurantByName(string name)
        {
            var para = new DynamicParameters();
            para.Add("@Name", name);
            return
                _Connection.Query<Restaurant>("[dbo].[GetRestaurantByName]", para, null, true, null,
                    CommandType.StoredProcedure).FirstOrDefault();
        }

        public List<Recommend> GetRecommend(int restaurantId)
        {
            return
                _Connection.Query<Recommend>(
                    "Select [Meal] as [Name], ABS([Cost]) as [Cost], [OrderCount] as [Count] from [Recommendation] where [RestaurantId] = @restId",
                    new { restId = restaurantId }).ToList();
        }

        public void AddIntoRecommendation(int restaurantId, OrderInfo order)
        {
            var para = new { restaurantId, order.Meal, order.Cost };
            _Connection.Execute("[dbo].[Add_Recommendation_16.03.01]", para, null, null, CommandType.StoredProcedure);
        }

        public int UpdateSummary(int customerId, string meal, int cost, int balance, string name)
        {
            var para = new DynamicParameters();
            para.Add("@CustomerId", customerId);
            para.Add("@Meal", meal);
            para.Add("@Cost", cost);
            para.Add("@Balance", balance);
            para.Add("@Name", name);
            para.Add("@Datetime", DateTime.Now);

            return _Connection.Query<int>("[dbo].[UpdateSummary]", para, null, true, null, CommandType.StoredProcedure)
                    .FirstOrDefault(); //FirstOrDefault() 取第一個資料或是NULL
        }

        public int InsertTransferRequest(int customerId, int requestToCustId, string meal, int cost)
        {
            var para = new DynamicParameters();
            para.Add("@CustomerId", customerId);
            para.Add("@RequestToCustId", requestToCustId);
            para.Add("@Meal", meal);
            para.Add("@Cost", cost);
            para.Add("@Datetime", DateTime.Now);

            return _Connection.Query<int>("[dbo].[InsertTransferRequest]", para, null, true, null, CommandType.StoredProcedure)
                    .FirstOrDefault(); //FirstOrDefault() 取第一個資料或是NULL
        }

        public int UpdateTransferSettle(int orderId)
        {
            var para = new DynamicParameters();
            para.Add("@orderId", orderId);
            return _Connection.Execute("UPDATE [Order] SET [Settle] = 'true' WHERE OrderId = @orderId", para) > 0 ? 0 : 1;
        }


        public int UpdateTransferInfo(int orderId, int transferStatus)
        {
            var para = new DynamicParameters();
            para.Add("@orderId", orderId);
            para.Add("@transferStatus", transferStatus);
            para.Add("@ModifyOn", DateTime.Now);
            return _Connection.Execute("UPDATE [TransferInfo] SET [TransferStatus] = @transferStatus,[ModifyOn] = @ModifyOn WHERE OrderId = @orderId AND [TransferStatus] = 1", para) > 0 ? 0 : 1;
        }

        public IEnumerable<Customer> GetUserSlackId(string name)
        {
            var slackId = _Connection.Query<Customer>("SELECT * FROM [Customer] WHERE Name = @name", new { name }).ToList();
            return slackId;
        }

        public void UpdateSlackId(string slackId,string user)
        {
            _Connection.Execute("UPDATE [Customer] SET [SlackId] = @slackId WHERE [Name] = @user",new { slackId,user});
        }

        public Customer GetCustomerBySalckId(string slackId)
        {
            return _Connection.Query<Customer>("SELECT * FROM [Customer] WHERE SlackId = @slackId", new { slackId }).FirstOrDefault();
        }

        public IEnumerable<OrderInfo> GetUserOrder(string name)
        {
            return _Connection.Query<OrderInfo>(@"SELECT distinct [Meal],[Cost],[OrderId],[Order].[Time],[Order].[RestaurantId]
     FROM[Order],[Customer],[Restaurant]
     WHERE[Order].CustomerId =[Customer].CustomerId AND[Settle] = 'false' AND OrderStatus = 'order' AND [Restaurant].[IsClose] = 0 AND [Restaurant].[RestaurantId] = [Order].[RestaurantId] AND [Customer].[Name] = @name
     Order by [Order].RestaurantId", new { name }).ToList();
        }
        public IEnumerable<Customer> GetAllCustomer()
        {
            return _Connection.Query<Customer>(@"select customerId,Name,Password from Customer", new { }).ToList();
        }

        public void SaltCustomerPassword(string password,int customerId)
        {
            _Connection.Execute("update customer set password=@password where CustomerId=@customerId",new{password, customerId });
        }
        public void ResetUserPassword(int customerid,string password)
        {
            _Connection.Execute("update Customer set Password=@Password where CustomerId=@CustomerId",new { Password=password,CustomerId=customerid});
        }
    }
}