# Auditor

[![License](https://img.shields.io/github/license/JoeBiellik/auditor.svg)](LICENSE.md)
[![Build Status](https://img.shields.io/appveyor/ci/JoeBiellik/auditor/master.svg)](https://ci.appveyor.com/project/JoeBiellik/auditor)
[![Release Version](https://img.shields.io/github/release/JoeBiellik/auditor/all.svg)](https://github.com/JoeBiellik/auditor/releases)

**auditor** — cross platform CLI file system linter and auditor

## Synopsis

**auditor** _rulebook.yml_ \[**-t**|**--target** _directory..._] \[**-e**|**--exclude** _pattern..._] \[**-j**|**--jobs** _num_] \[**-o**|**--out** _file_] \[**-v**|**--verbose**|**-q**|**--quiet**]

**auditor** **--checks** \[**-v**|**--verbose**|**-q**|**--quiet**]

**auditor** \[**--help**|**--version**]

## Description

Audits and lints files and directories, validating them against checks according to a YAML rulebook.

## Arguments

* _rulebook.yml_

  The name of a YAML format file to run as a rulebook.

## Options

* **-t** _directory..._, **--target** _directory..._

  The path to one or more directories to run the checks from.

  Takes priority over directories value from the rulebook if specified.

  Targets can be provided via standard input, one path per line.

* **-e** _pattern..._, **--exclude** _pattern..._

  One or more patterns to exclude from targeting.

  Takes priority over exclude value from the rulebook if specified.

* **-j** _num_, **--jobs** _num_

  Number of jobs to run simultaneously, defaults to system CPU thread count.

  A value of 1 will guarantee checks run in a consistent order.

* **-o** _file_, **--out** _file_

  File to write check results to.

* **--checks**

  Prints available check information and exits.

* **-v**, **--verbose**

  Enable verbose output.

  Takes priority over `--quiet`.

* **-q**, **--quiet**

  Enable quiet output.

* **--help**

  Prints brief usage information and exits.

* **--version**

  Prints the current version number and exits.

## Examples

### Standard usage

```shell
auditor rules.yml
```

### Override rulebook targets

```shell
auditor rules.yml --target D:\Documents E:\
```

### Override rulebook targets from STDIN

```shell
echo D:\ | auditor rules.yml
```

### Override rulebook target excludes

```shell
auditor rules.yml --exclude "**/*.txt"
```

## Checks

Checks target either a file or a directory, perform an action and return success or failure according to the requirements.

Checks are loaded and resolved at runtime from any compatible `.dll` file found in the `Checks/` directory relative to the application binary.

## Rulebook Format

Auditor uses YAML rulebook files to define check rules. A rulebook is consists of the following basic properties:

```yaml
directory:
# Windows
- 'D:\'
- '%USERPROFILE$\Documents'
- '\\server\share\Files\'
# Linux/MacOS
- '/mnt/docs/'
- 'files/'
- '~/documents'

exclude: '*.txt'

rules:
  ...
```

### directory

#### Optional, path or list of paths to target

Paths can be absolute or relative to the working directory, in any format the operating system supports.

If directories are specified at runtime, the rulebook value is ignored.

### exclude

#### Optional, path or list of paths to exclude from target

Paths can be absolute or relative to the working directory, in any format the operating system supports.

If directories are specified at runtime, the rulebook value is ignored.

### rules

#### List of check rules to run

List of check rules to run.

## Rules Format

```yaml
rules:
- name: Images are below 1MB in size
  target:
    - '**\*.+(jpg|png|bmp)'  # Any .jpg .png or .bmp files anywhere under the target
    - '!**\folder.jpg'       # Exclude any folder.jpg files
  auditor.checks.file.size:
    max: 1mb
```

### name

#### Display name string

The display name of this check instance

Should be a descriptive name summarising the characteristics this check is verifying.

### target

#### List of target patterns

### [check]

#### Name of check

The name of the check to run, corresponding to the available check plugins at runtime.

See `--checks` to print a list of available checks.

Corresponds to the full type name of the check as lowercase.

### [check].[property]

#### Check option

A check property can be any name and require a value of any type, as defined by the selected check. See the individual check documentation for its options.

## Pattern

Auditor uses Minimatch patterns to include or exclude targets with some extra features.

```text
**\*.+(jpg|png|bmp)
```

A pattern is a negative match if it starts with an `!`.

```text
!**\              # Exclude all directories
!file-{1..5}.txt  # Exclude a range of files
!image.jpg        # Exclude a single file
```

A pattern can have extra flags attached to it by ending with a `?` followed by flag symbols.

### Flags

* **i**

  Make the pattern match case insensitive.

  ```text
  *.jpg?i
  ```

* **r**

  Parse the pattern as a regular expression rather than with Minimatch.

  ```text
  ^\w+\.txt$?r
  \.txt$?ri     # Case insensitive regex
  ```

## Included Checks

### Directory Checks

#### Exists

> Auditor.Checks.Directory.Exists

#### Empty

> Auditor.Checks.Directory.Empty

#### Contains

> Auditor.Checks.Directory.Contains

#### Max Extensions

> Auditor.Checks.Directory.MaxExtensions

### File Checks

#### Empty

> Auditor.Checks.File.Empty

#### Matches

> Auditor.Checks.File.Matches

#### Size

> Auditor.Checks.File.Size

### Audio Checks

#### Bitrate

> Auditor.Checks.Audio.Bitrate

#### Channels

> Auditor.Checks.Audio.Channels

#### Sample Rate

> Auditor.Checks.Audio.SampleRate

#### Tag File Count

> Auditor.Checks.Audio.TagFileCount

#### Tag Matches

> Auditor.Checks.Audio.TagMatches

#### Tags Match

> Auditor.Checks.Audio.TagsMatch

#### Path

> Auditor.Checks.Audio.Path

#### MPEG VBR

> Auditor.Checks.Audio.MpegVbr

#### Frames

> Auditor.Checks.Audio.Frames

#### Image

> Auditor.Checks.Audio.Image

#### Image Count

> Auditor.Checks.Audio.ImageCount

#### Embedded Art Extracted

> Auditor.Checks.Audio.EmbeddedArtExtracted

### Image Checks

#### Aspect Ratio

> Auditor.Checks.Image.AspectRatio

#### Resolution

> Auditor.Checks.Image.Resolution

#### File Interlace

> Auditor.Checks.Image.FileInterlace

#### File Format

> Auditor.Checks.Image.FileFormat

#### File Compression

> Auditor.Checks.Image.FileCompression

#### File Colorspace

> Auditor.Checks.Image.FileColorspace

### Video Checks

#### Streams

> Auditor.Checks.Video.Streams

#### Audio Streams

> Auditor.Checks.Video.AudioStreams

#### Video Streams

> Auditor.Checks.Video.VideoStreams

#### Video Format

> Auditor.Checks.Video.VideoFormat
