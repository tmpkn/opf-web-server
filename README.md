opf-web Introduction
--------------------

opf-web is a fork of [OpenPasswordFilter](https://github.com/jephthai/OpenPasswordFilter), which posts password change requests and update notifications to a web app using HTTP. This is not meant to be an out of the box solution, but rather a reference frame to implement your custom library which meets your business requirements.

More information: https://tompaw.net/opf-web/

Quick & Dirty Installation
--------------------------

1) You will need Visual Studio 2022 (CE is fine) to build this code.
2) Edit App.config and update the URLs for your password filter and notification endpoints. This should probably be in Windows registry.
3) Build the solution, grab the following two files: `OpenPasswordFilter.dll` and `OPFService.exe`. You will need those files on every master (rw) AD controller in your domain.
4) Put `OpenPasswordFilter.dll` in `%WINDIR%\System32` and `OPFService.exe` in a location of your choice, for instance `C:\opf\`
5) Add `OpenPasswordFilter` to `HKLM\SYSTEM\CurrentControlSet\Control\Lsa\Notification Packages` of the AD controller.
6) Assuming you put the service binary in `C:\opf\`, register the service by using this command verbatim (including spaces):

    > sc create OPF binPath= "c:\opf\opfservice.exe" start= boot

7) At this point, all password changes should be passed through your newly installed DLL & service. Make sure to deploy the web app: DC must be able to send HTTP requests to the URLs you defined in step (2). 

A sample implementation of the web app is available here:

xxx

Links
-----

Original OpenPasswordFilter: https://github.com/jephthai/OpenPasswordFilter
