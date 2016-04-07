# Vostok
Windows auto-installable service for send daily backup to ftp server

For configuration, edit file palace.exe.config

<appSettings>
	<add key="RootFolder" value=".\" />
	<add key="Pattern" value="*.bak" />
	<add key="Ftp" value="ftp://myftp.com/" />
	<add key="Login" value="" />
	<add key="Password" value="" />
	<add key="UseBinary" value="true" />
	<add key="UsePassive" value="true" />
	<add key="KeepAlive" value="true" />
	<add key="BufferSize" value="4096" />
</appSettings>

install :

> palace.exe /intall

unstall : 

> palace.exe /uninstall



