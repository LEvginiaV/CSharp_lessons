"use strict";

const ts = require("typescript");
const fs = require("fs");
const path = require("path");
const process = require("process");
const fse = require("fs-extra");
const glob = require("glob");

function reportDiagnostics(diagnostics) {
  diagnostics.forEach(diagnostic => {
    let message = "Error";
    if (diagnostic.file) {
      const where = diagnostic.file.getLineAndCharacterOfPosition(diagnostic.start);
      message += " " + diagnostic.file.fileName + " " + where.line + ", " + where.character + 1;
    }
    message += ": " + ts.flattenDiagnosticMessageText(diagnostic.messageText, "\n");
    console.log(message);
  });
}

function readConfigFile(configFileName) {
  // Read config file
  const configFileText = fs.readFileSync(configFileName).toString();

  // Parse JSON, after removing comments. Just fancier JSON.parse
  const result = ts.parseConfigFileTextToJson(configFileName, configFileText);
  const configObject = result.config;
  if (!configObject) {
    reportDiagnostics([result.error]);
    process.exit(1);
  }

  // Extract config infromation
  const configParseResult = ts.parseJsonConfigFileContent(configObject, ts.sys, path.dirname(configFileName));
  if (configParseResult.errors.length > 0) {
    reportDiagnostics(configParseResult.errors);
    process.exit(1);
  }
  return configParseResult;
}


function compile(configFileName) {
  // Extract configuration from config file
  const config = readConfigFile(configFileName);

  // Compile
  const program = ts.createProgram(config.fileNames, config.options);
  const emitResult = program.emit();

  // Report errors
  reportDiagnostics(ts.getPreEmitDiagnostics(program).concat(emitResult.diagnostics));

  // Return code
  const exitCode = emitResult.emitSkipped ? 1 : 0;
  if (exitCode !== 0) {
    console.error("Error while build. Exit code " + exitCode + ".");
    process.exit(exitCode);
  }
}

function deleteFolderRecursive(path, includeDirectory = false) {
  if (fs.existsSync(path)) {
    fs.readdirSync(path).forEach(function(file, index) {
      var curPath = path + "/" + file;
      if (fs.lstatSync(curPath).isDirectory()) { // recurse
        deleteFolderRecursive(curPath, true);
      } else { // delete file
        fs.unlinkSync(curPath);
      }
    });
    if (includeDirectory)
      fs.rmdirSync(path);
  }
}

function getFilesAsync(from, template) {
  return new Promise(resolve => {
    glob(template, { cwd: from }, (er, files) => {
      if (er !== null) {
        console.error("Error while trying to get files", template, er);
        process.exit(1);
      }
      resolve(files);
    });
  });
}

async function copyFiles(source, destination, excludeRules, includeRules) {
  const excludeArrays = excludeRules.map(x => getFilesAsync(source, x));
  const excludeFilePaths = [].concat(...(await Promise.all(excludeArrays)));
  const excludeFilePathSet = new Set();
  excludeFilePaths.map(x => excludeFilePathSet.add(x));

  const includeArrays = includeRules.map(x => getFilesAsync(source, x));
  let copyFilePaths = [].concat(...(await Promise.all(includeArrays))).filter(x => !excludeFilePathSet.has(x));
  copyFilePaths = [...new Set(copyFilePaths)];
  console.log(`Found ${copyFilePaths.length} files:`);
  copyFilePaths.forEach(x => console.log(x));

  copyFilePaths.forEach(x => fse.copySync(path.join(source, x), path.join(destination, x)));
}

async function doAll() {
  try {
    console.log("removing dist...");
    await fse.remove("./dist");

    console.log("removing alco.customers-staff-ui/dist/...");
    await fse.remove("./../../../alco.customers-staff-ui/dist/");

    console.log("compliling...");
    compile("tsconfig.json");

    console.log("copying js, jsx, less, png, svg... ");
    await copyFiles(
      "./src",
      "./../../../alco.customers-staff-ui/dist/",
      [
        "**/*.stories.*",
      ],
      [
        "./**/*.js",
        "./**/*.jsx",
        "./**/*.less",
        "./**/*.png",
        "./**/*.svg",
      ]);

    console.log("copying dist... ");
    await copyFiles(
      "./dist",
      "./../../../alco.customers-staff-ui/dist/",
      [
        "**/*.stories.*",
      ],
      [
        "**/*",
      ]);

    console.log("copying package.json");
    await fse.copy("./package.json", "./../../../alco.customers-staff-ui/package.json");

    console.log("removing dist...");
    await fse.remove("./dist");

    console.log("finished!");
  }
  catch(e) {
    console.error("Error", e);
    process.exit(1);
  }
}

doAll();
