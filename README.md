# AbotX [![NuGet](https://img.shields.io/nuget/v/Abotx.svg)](https://www.nuget.org/packages/Abotx/)

A powerful C# web crawler that makes advanced crawling features easy to use. AbotX builds upon the open source Abot C# Web Crawler by providing a powerful set of wrappers and extensions.

*Please star this project!!* Contact me with exciting opportunities!!

* Install AbotX using [Nuget](https://www.nuget.org/packages/AbotX/)
* Go to the [AbotX website](https://abotx.org) for more information and tutorials

### Release Notes ###
  * [v1.3.53](https://www.nuget.org/packages/AbotX/1.3.53) released 2017-07-16
    * Removal of classes/methods marked obsolete (CSQuery support, extra constructor params, etc..)
    * Removal of dependence on Commoner.Core in several areas
    * Update of AbotX.Agent to use a much newer version of unity
    * Update of AbotX.Agent to use a more robust IOC injection configuration schema that allows config based injection of all impls (Abotx and Abot)
  * [v1.2.130](https://www.nuget.org/packages/AbotX/1.2.130) released 2017-07-16
    * Fixed issue with javascript rendering even if decision was false, [More details here](https://github.com/sjdirect/abotx/issues/15)
  * [v1.2.126](https://www.nuget.org/packages/AbotX/1.2.126) released 2017-07-01
    * Fixed issue with null reference if ImplementationContainer was passed in, [More details here](https://github.com/sjdirect/abotx/issues/14
  * [v1.2.124](https://www.nuget.org/packages/AbotX/1.2.124) released 2017-04-03
    * Updated Abot dependency to the latest version
  * [v1.2.123](https://www.nuget.org/packages/AbotX/1.2.123) released 2017-02-12
    * Fixed cookie handling tranfer from AbotX to PhantomJs bug, [More details here](https://github.com/sjdirect/abotx/issues/13).
  * [v1.2.120](https://www.nuget.org/packages/AbotX/1.2.120) released 2017-01-08
    * Removal of Automapper dependency
    * Upgraded depdendent lib versions to latest
    * Minor bug fixes
  * [v1.2.71](https://www.nuget.org/packages/AbotX/1.2.71) released 2017-01-04
    * Javascript rendering stability improvement
  * [v1.2.48](https://www.nuget.org/packages/AbotX/1.2.48) released 2016-09-18
    * [Issue #11](https://github.com/sjdirect/abotx/issues/11) The DateTime represented by the string is out of range
  * [v1.2.46](https://www.nuget.org/packages/AbotX/1.2.46) released 2016-08-25
    * [Issue #10](https://github.com/sjdirect/abotx/issues/10) Access to the registry key 'Global' is denied
  * [v1.2.44](https://www.nuget.org/packages/AbotX/1.2.44) released 2016-08-18
    * [Issue #9](https://github.com/sjdirect/abotx/issues/9) Partial fix 3/3 (more null references with new constructors
  * [v1.2.42](https://www.nuget.org/packages/AbotX/1.2.42) released 2016-08-17
    * Added checks to make sure Auto Tuning speed up cannot override robots.txt crawl delay
    * Fixed CrawlerX.SpeedUp not respecting all config values bug
    * Fixed Null reference when AutoThrottling/Tuning configs are missing bug
  * [v1.2.33](https://www.nuget.org/packages/AbotX/1.2.33) released 2016-08-17
    * [Issue #9](https://github.com/sjdirect/abotx/issues/9) Partial fix 2/3
  * [v1.2.32](https://www.nuget.org/packages/AbotX/1.2.32) released 2016-08-17
    * [Issue #9](https://github.com/sjdirect/abotx/issues/9) Partial fix 1/2
  * [v1.2.28](https://www.nuget.org/packages/AbotX/1.2.28) released 2016-08-16
    * Marking methods and constructors Obsolete
    * Minor bug fixes to tuning
  * [v1.2.2](https://www.nuget.org/packages/AbotX/1.2.2) released 2016-08-16
    * [New Auto Throttling feature](https://abotx.org/Learn/AutoThrottling) (beta)
    * [New Auto Tuning feature](https://abotx.org/Learn/AutoTuning) (beta)
    * Minor bug fixes and enhancements to existing functionality

### Example Code & Tutorial ###
Executable code samples and tutorials will soon be available on this repo. For now you can see these at the [AbotX website](https://abotx.org).
