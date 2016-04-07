# Vostok
Windows auto-installable service for send daily backup to ftp server

Pour la configuration, il faut éditer le fichier palace.exe.config

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

RootFolder indique le repertoire ou se trouvent les fichiers à sauvegarder, tous les fichiers portant l'extention indiquée dans le paramètre "Pattern" seront envoyés
dans le répertoire distant indiqué par le paramètre "Ftp".

Vostok synchronise le repertoire distant, si un fichier est présent sur celui-ci et n'est pas présent localement il sera supprimé dans le repertoire distant.

La synchronisation s'effectue par défaut tous les jours à 5h, pour modifier ce paramètre il suffit d'indiquer l'heure dans "StartHour"

Les autres paramètres servent pour l'autentification et la configuration de la vitesse.

Pour installer le service :

> palace.exe /install

Pour désinstaller le service : 

> palace.exe /uninstall


A noter, si Palace.exe est lancé sans paramètre il sera executé en mode console.


