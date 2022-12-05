from line import LineClient, LineGroup, LineContact

try:
    client = LineClient(authToken="DUwK2g4prnzWQsEgqzh7.yU5Cj6MlKBJ/CNpE9/aqHW.2ZM4gd71SH1aVEtrcYktj+W6M2FKYaTJo6kQjggAz+M=")
except:
    print "Login Failed"

group = client.getGroupByName("Test")
group.sendMessage("Hi, the menu has been pushed~ Please go to http://lunch.titansoft.com to order your lunch~ Have a nice day!")

