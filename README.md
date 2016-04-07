# Vostok
Windows auto-installable service for send daily backup to ftp server

Pour la configuration, il faut �diter le fichier palace.exe.config

```xml
<appSettings>
	<add key="RootFolder" value=".\" />
	<add key="Pattern" value="*.bak" />
	<add key="Ftp" value="ftp://myftp.com/folder" />
	<add key="Login" value="" />
	<add key="Password" value="" />
	<add key="UseBinary" value="true" />
	<add key="UsePassive" value="true" />
	<add key="KeepAlive" value="true" />
	<add key="BufferSize" value="4096" />
	<ass key="StartHour" value="5" />
</appSettings>
```

RootFolder indique le repertoire ou se trouvent les fichiers � sauvegarder, tous les fichiers portant l'extention indiqu�e dans le param�tre "Pattern" seront envoy�s
dans le r�pertoire distant indiqu� par le param�tre "Ftp".

Vostok synchronise le repertoire distant, si un fichier est pr�sent sur celui-ci et n'est pas pr�sent localement il sera supprim� dans le repertoire distant.

La synchronisation s'effectue par d�faut tous les jours � 5h, pour modifier ce param�tre il suffit d'indiquer l'heure dans "StartHour"

Les autres param�tres servent pour l'autentification et la configuration de la vitesse.

Pour installer le service :

> palace.exe /install

Pour d�sinstaller le service : 

> palace.exe /uninstall


A noter, si Palace.exe est lanc� sans param�tre il sera execut� en mode console.


