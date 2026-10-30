# BFP Fire System Checklist Inspection (with E-Signature) — Giya sa Bisaya

## Ang dagan sa app (flow)

```
PAGE 1  MainPage            -> Inspections button
PAGE 2  DashboardPage       -> search bar, 4 ka indicator, listahan nga naay progress bar
          + New inspection  -> ChecklistTypesPage (dagkong button matag form)
                            -> NewInspectionPage (ngalan, address, owner, contact, IO no., petsa)
PAGE 3  FormPage            -> sagutan ang checklist (autosave)
          Save draft        -> balik sa dashboard (mabalikan bisan kanus-a)
          Next missing      -> pangitaon ang kulang
          Sign-off          -> SignoffPage -> SignaturePage (pirma gamit ang tudlo)
                            -> Complete inspection (ma-lock)
          Fire Marshal      -> mo-pirma sa opisina (Approved / Disapproved)
```

## Ang status

| Status | Kahulugan |
|---|---|
| Draft | Wala pa nahuman ang form |
| Missing signature | Nahuman ang form pero kulang ang pirma |
| Awaiting approval | Na-complete ug na-lock, naghulat sa Fire Marshal |
| Completed | Naka-pirma na ang Fire Marshal |

## Asa ang code

| Gusto nimo usbon | File |
|---|---|
| Dashboard (page 2) | Views/DashboardPage.xaml, ViewModels/DashboardPageViewModel.cs |
| Form (page 3) | Views/FormPage.xaml, ViewModels/FormPageViewModel.cs |
| Progress bar nga ihap | Services/ProgressCalculator.cs |
| Pirma | Views/SignaturePage.xaml(.cs), Controls/SignatureDrawables.cs |
| Sign-off ug Complete | Views/SignoffPage.xaml, ViewModels/SignoffPageViewModel.cs |
| Database nga trabaho | Services/InspectionService.cs ug InspectionService.Workflow.cs |
| Sulod sa mga form | Resources/Raw/seeds/*.sql |
| Disenyo sa database | Resources/Raw/schema.sql |

## Hinumdumi
- Matag usab sa schema.sql o seeds: i-delete ang `bfp_inspection.db3` (o i-uninstall ang app).
- Ang tanang file naay Bisaya nga comment sa ibabaw ug sa importanteng mga linya.
