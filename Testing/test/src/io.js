// write + read
assert(() => IO.writeText("io/a.txt", "testvalue38") == undefined, "IO.write");
assert(() => IO.readText("io/a.txt") == "testvalue38", "IO.read");

// copy file
assert(() => IO.copyFile("io/a.txt", "io/b.txt") == undefined, "IO.copy");
assert(() => IO.readText("io/b.txt") == "testvalue38", "IO.copy new file");

// delete file
assert(() => IO.deleteFile("io/a.txt") == undefined, "IO.delete");
assert(() => !IO.fileExists("io/a.txt"), "IO.delete gone");

// move file
assert(() => IO.moveFile("io/b.txt", "io/c.txt") == undefined, "IO.move");
assert(() => !IO.fileExists("io/b.txt"), "IO.move old gone");
assert(() => IO.readText("io/c.txt") == "testvalue38", "IO.move new file");

IO.deleteFile("io/c.txt");

// create directory
assert(() => IO.createDirectory("io/documents") == undefined, "IO.createDirectory");
assert(() => IO.directoryExists("io/documents"), "IO.createDirectory exists");

// move directory
assert(() => IO.moveDirectory("io/documents", "io/pictures") == undefined, "IO.moveDirectory");
assert(() => !IO.directoryExists("io/documents"), "IO.moveDirectory old gone");
assert(() => IO.directoryExists("io/pictures"), "IO.moveDirectory new exists");

// delete directory
assert(() => IO.deleteDirectory("io/pictures") == undefined, "IO.deleteDirectory");
assert(() => !IO.directoryExists("io/pictures"), "IO.deleteDirectory gone");

// get files
var files = IO.getFiles("io/example");
assert(() => files.length == 2, "IO.getFiles count");
assert(() => files[0].endsWith("fileA.txt"), "IO.getFiles[0]");
assert(() => files[1].endsWith("fileB.png"), "IO.getFiles[1]");

// get directories
var directories = IO.getDirectories("io/example");
assert(() => directories.length == 2, "IO.getDirectories count");
assert(() => directories[0].endsWith("folderA"), "IO.getDirectories[0]");
assert(() => directories[1].endsWith("folderB"), "IO.getDirectories[1]");