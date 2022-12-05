import sys
from line import LineClient, LineGroup, LineContact

try:
    client = LineClient(authToken=str(sys.argv[1]))
except:
    print "Login Failed"

group = client.getGroupByName(str(sys.argv[2]))
group.sendMessage(str(sys.argv[3]))

