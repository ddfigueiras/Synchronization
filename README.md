# Synchronization
This program facilitates synchronization between two specified directories. It operates based on the provided command-line arguments.

# Usage
To use the synchronization feature, follow the format below:
```
    Copy "source path" "replica path" "[Optional <log path>]"
```
"source path": Path to the source directory.
"replica path": Path to the replica directory.
"[Optional <log path>]": (Optional) Path to the log file. If not provided, the default log file location will be used.


# Example
```
    Copy "C:\Source" "D:\Replica" "C:\Logs\SyncLog.txt"
```
This command synchronizes the contents of the "C:\Source" directory with the "D:\Replica" directory and logs the synchronization process to the specified log file at "C:\Logs\SyncLog.txt".

For any assistance or issues, please refer to the program documentation or contact the developer.
