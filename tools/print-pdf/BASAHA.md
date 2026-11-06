# Print PDF tools

Kini ang naghimo sa `Resources/Raw/print/NNN.pdf` ug `NNN.json` gikan sa `NNN.docx` / `NNN.xlsx` (ang mga form nga naay `{{marker}}`).
Dili ni apil sa app. Gamita lang kung giusab ang usa ka form.

Kinahanglan: Python 3, LibreOffice (`soffice`), ug `pip install lxml pdfplumber pikepdf`.

```
python pdf_build.py 064 "../fsi checklist/Resources/Raw/print/064.docx" out
```

Mogawas sa `out/`: `064.pdf` (blangko nga form) ug `064.json` (asa ibutang ang tubag).
Kopyaha ang duha ngadto sa `fsi checklist/Resources/Raw/print/`.

Ang `pdf_fill.py` mao ang kaparehas sa `PrintService.cs` sa Python (para tan-awon ang resulta sa computer).
