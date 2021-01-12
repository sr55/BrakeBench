# BrakeBench ![Build](https://github.com/sr55/BrakeBench/workflows/Build/badge.svg)
A multi-platform benchmark and analysis tool for HandBrake


## Usage

* Setup your benchmark profiles in config.json
  * TaskId must be unique. This is what will be used to execute this task at the command line.
  * It is recommended to run the same job multiple times. When this is done, the average results will also be automatically calculated. 

* Place your source files in a folder called "sources" in the same directory as the BrakeBench Executable. 

* Execute brakebench from hte command line:
  * On Windows:  BrakeBench <TaskId>
  * On Linux:    ./BrakeBench <TaskId>
  * On macOS     ./BrakeBench <TaskId>
  
  TaskId related to the task id configured in your config file.


## Bug Reports and Feature Requests.

Please use the Issues tab above to report any issues or feature requests. 

## License

BrakeBench is relased under the [3-Clause BSD License](https://github.com/sr55/BrakeBench/blob/master/LICENSE) 
