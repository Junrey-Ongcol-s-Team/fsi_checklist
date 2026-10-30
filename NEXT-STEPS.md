# Next session: photo attachments

Status (26 Oct 2026): app builds and runs on Windows. Dashboard, forms (10), sign-off, e-signature,
View results, AppLog (app.log), and Nav helper all work.

## To build next
1. Map the `attachment` table in Data/Models/Entities.cs + AppDbContext.
2. Form page: "Take photo" / "Choose photo" per PHOTO item (064 item 13: FRONT, REAR, SIDE slots
   from form_choice) and "Add photo" for the defects sections (061, 069, 024, 021, 026).
3. Save files to AppData/.../Data/photos/, record rows in `attachment` (inspection_id, item_id, tag).
4. Thumbnails + Remove button; block add/remove when the inspection is COMPLETED.
5. Count 064's three photos in ProgressCalculator.
6. Show photos in View results.
7. Permissions: Android CAMERA (AndroidManifest.xml), iOS NSCameraUsageDescription (Info.plist).

## Rule from the owner
- KEEP THE CODE SIMPLE. Small, plain changes that are easy to read and debug.
  No extra layers, libraries or clever patterns. Add Bisaya comments.

## Reminders
- Use F5 (Windows Machine), not Rebuild Solution (Android case_map.txt lock is harmless).
- Don't use RelativeSource AncestorType in item templates: use code-behind handlers (see DashboardPage.xaml.cs).
- Never delete bfp_inspection.db3-wal / -shm.
