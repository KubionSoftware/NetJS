async function testIO(){
	// write + read text
	let writeTextResult = await IO.writeText("io/a.txt", "testvalue38");
	Test.assert(() => writeTextResult === true, "IO.writeText");
	let readTextResult = await IO.readText("io/a.txt");
	Test.assert(() => readTextResult === "testvalue38", "IO.readText");

	// write + read bytes
	let bytes = new Uint8Array(1);
	bytes[0] = 42;
	let writeBytesResult = await IO.writeBytes("io/bytes.txt", bytes);
	Test.assert(() => writeBytesResult === true, "IO.writeBytes");
	let readBytesResult = await IO.readBytes("io/bytes.txt");
	Test.assert(() => readBytesResult[0] === 42, "IO.readBytes");

	// copy file
	let copyFileResult = await IO.copyFile("io/a.txt", "io/b.txt");
	Test.assert(() => copyFileResult === true, "IO.copy");
	let copiedFileText = await IO.readText("io/b.txt");
	Test.assert(() => copiedFileText === "testvalue38", "IO.copy new file");

	// delete file
	let deleteFileResult = await IO.deleteFile("io/a.txt");
	Test.assert(() => deleteFileResult === true, "IO.delete");
	let deletedFileGone = await IO.fileExists("io/a.txt");
	Test.assert(() => deletedFileGone === false, "IO.delete gone");

	// move file
	let moveFileResult = await IO.moveFile("io/b.txt", "io/c.txt");
	Test.assert(() => moveFileResult === true, "IO.move");
	let movedFileGone = await IO.fileExists("io/b.txt");
	Test.assert(() => movedFileGone === false, "IO.move old gone");
	let movedFileText = await IO.readText("io/c.txt");
	Test.assert(() => movedFileText === "testvalue38", "IO.move new file");

	await IO.deleteFile("io/c.txt");

	// create directory
	let createDirectoryResult = await IO.createDirectory("io/documents");
	Test.assert(() => createDirectoryResult === true, "IO.createDirectory");
	let createdDirectoryExists = await IO.directoryExists("io/documents");
	Test.assert(() => createdDirectoryExists === true, "IO.createDirectory exists");

	// move directory
	let moveDirectoryResult = await IO.moveDirectory("io/documents", "io/pictures");
	Test.assert(() => moveDirectoryResult === true, "IO.moveDirectory");
	let movedDirectoryGone = await IO.directoryExists("io/documents");
	Test.assert(() => movedDirectoryGone === false, "IO.moveDirectory old gone");
	let movedDirectoryExists = await IO.directoryExists("io/pictures");
	Test.assert(() => movedDirectoryExists === true, "IO.moveDirectory new exists");

	// delete directory
	let deleteDirectoryResult = await IO.deleteDirectory("io/pictures");
	Test.assert(() => deleteDirectoryResult === true, "IO.deleteDirectory");
	let deletedDirectoryExists = await IO.directoryExists("io/pictures");
	Test.assert(() => deletedDirectoryExists === false, "IO.deleteDirectory gone");

	// get files
	let files = await IO.getFiles("io/example");
	Test.assert(() => files.length == 2, "IO.getFiles count");
	Test.assert(() => files[0].endsWith("fileA.txt"), "IO.getFiles[0]");
	Test.assert(() => files[1].endsWith("fileB.png"), "IO.getFiles[1]");

	// get directories
	let directories = await IO.getDirectories("io/example");
	Test.assert(() => directories.length == 2, "IO.getDirectories count");
	Test.assert(() => directories[0].endsWith("folderA"), "IO.getDirectories[0]");
	Test.assert(() => directories[1].endsWith("folderB"), "IO.getDirectories[1]");
}