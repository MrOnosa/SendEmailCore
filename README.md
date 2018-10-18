# SendEmailCore
A simple command line SMTP email client built in dotnet core

This command requires two thing: 1. A from address. 2. A message. The message can be provided with either the -m command or piped in via STDIN.

Usage examples  
* `dotnet SendEmailCore.dll -h smtp.gmail.com -p 587 -xu YourEmail@gmail.com -xp YOURPAS$WORD -f YourEmail@gmail.com -t SomeoneYouAreEmailing@domain.com -t SomeoneElseYouAreEmailing@domain.com -s tacos -m 'Bring the tacos'` 
* `echo 'But where is the dip?' | dotnet SendEmailCore.dll -h smtp.gmail.com -p 587 -xu YourEmail@gmail.com -xp YOURPAS$WORD -f YourEmail@gmail.com -bcc SomeoneYouAreEmailing@domain.com -s 'I see the chip' --trace:verbose`

| Options | Description 
--- | --- 
  --version                  | Show version information
  -?\|--help                  | Show help information
  -f\|--from \<FROM>           | The email address of the sender. Required
  -t\|--to \<TO>               | The email address of the receiver(s)
  -s\|--subject \<SUBJECT>     | Message subject
  -m\|--message \<MESSAGE>     | Message body. STDIN could be used instead
  -h\|--host \<HOST>           | SMTP client host, default is localhost
  -p\|--port \<PORT>           | SMTP client port, default is 25
  -cc\|--cc \<CC>              | The email address of the cc receiver(s)
  -bcc\|--bcc \<BCC>           | The email address of the bcc receiver(s)
  -xu\|--username \<USERNAME>  | Username to use for authentication, like an email address
  -xp\|--password \<PASSWORD>  | Password to use for authentication
  -T\|--trace \<TRACE>         | See trace messages. --trace will display info level messages. --trade:verbose will display verbose and info level messages
  --disable-ssl              | Specify the SMTP email client should not use Secure Sockets Layer (SSL) to encrypt the connection

## To the cloners

This is a simple dotnet core project, so getting started is easy. If you prefer the command line, which is likely given the nature of this project, run these commands in the SendEmailCore project folder:  
`dotnet restore`  
`dotnet build`  
`dotnet bin\Debug\netcoreapp2.1\SendEmailCore.dll --help`