-- =====================================================================
-- BISAYA: N/A NGA LAGDA sa TANANG FORMS.
-- Kung "N/A" ang gipili sa Passed / Failed / N/A sa usa ka pangutana nga naay numero
-- (pananglitan Doors ___ m), ang mga anak niini (pananglitan Capacity ___ persons) matago.
--   show_if_values = 'result:!NA'  ->  mogawas kung DILI N/A (apil kung wala pa'y tubag)
-- (Ang numero sa samang pangutana gitago sa app mismo: FormPageViewModel.ApplyNaRule.)
-- Modagan KAUSA RA (seed.na_rule).
-- =====================================================================

UPDATE form_item
   SET show_if_item_id = parent_item_id, show_if_values = 'result:!NA'
 WHERE show_if_item_id IS NULL
   AND parent_item_id IN (
       SELECT p.id FROM form_item p
        WHERE p.data_type IN ('NUMBER','INTEGER')
          AND EXISTS (SELECT 1 FROM form_column col
                        JOIN form_choice ch ON ch.column_id = col.id
                       WHERE col.column_set_id = p.column_set_id AND col.key = 'result' AND ch.value = 'NA'));
