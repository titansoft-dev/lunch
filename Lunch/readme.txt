1. Prepare environment
To use the push message to line function, need to install python 2.7 or other python 2.* higher then 2.7. Using python 3.* will not work.

After install, please add python.exe to the system environment path. If added successfully, open a new command line console(need to be new open for environment path to be loaded), and type python.
If the is python info message output there, then should be success.

Open the folder "Line-master" under the system project root folder. There should be a setup.py file. Use the command line, go to this folder, run "python setup.py install".

After install success, go to thrift folder under Line-master, run thrift.exe -r --gen py line.thrift

After before steps, the line client is ready to use.

2. How to get authToken of line
>python
>>>from line import LineClient
>>>client = LineClient("yourlineemail","password") #after doing this, there should show a message to ask you enter pin number, after enter will go to next step
>>>client.authToken #this will return the authToken ready for use

3. Scripts/LineClient.py
You could run this in command line, 
python LineClient.py {authToken} {groupName} {Message}

replace {authToken}, {groupName}, {Message} with the ones you need respectively.

4. Project setting
The configuration of Line is added in web config, below is the current setting. Please change accordingly.
    <add key="LineToken" value="DUwK2g4prnzWQsEgqzh7.yU5Cj6MlKBJ/CNpE9/aqHW.2ZM4gd71SH1aVEtrcYktj+W6M2FKYaTJo6kQjggAz+M=" />
    <add key="Group" value="Test" />
    <add key="IsLineEnabled" value="Y" />

