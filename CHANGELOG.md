
## Version Alpha 0.0.5 (2023-05-14) [PRIVATE]
* Added HTTP client for sending data.
* Added support for complex logging.
* Added OCR background service.
* Added ability to capture screenshots of data capture areas and save them, and send them to the OCR queue.
* Renamed `VectorA` and `VectorB` properties of the `CaptureArea` data model to `Location` and `Size`, respectively.
* Minor code cleanups.

## Version Alpha 0.0.4 (2023-05-13) [PRIVATE]
* Fixed issue with filesystem watcher triggering on intended file changes.

## Version Alpha 0.0.3 (2023-05-13) [PRIVATE]
* Added filesystem watcher to track unintended file changes.
* Added links to Github repository and changelog in the welcome page.
* Added version number to title bar.
* Added tooltip to "Share feedback" button indicating that it redirects to the author's Twitter profile.
* Improved formatting of CHANGELOG.md.
* Fixed positioning of information bars.
* Fixed a potential deadlock in the `ReloadCaptureAreasCommand`.

## Version Alpha 0.0.2 (2023-05-13) [PRIVATE]
* Added filters to the capture areas list.
* Added form validation for capture areas.
* Added copyright notice throughout to comply with the MIT license.

## Version Alpha 0.0.1 (2023-05-12) [PRIVATE]
* Initial release.
