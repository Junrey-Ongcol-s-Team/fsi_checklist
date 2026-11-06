-- =====================================================================
-- BISAYA: Inspection During Construction (068) I. MEANS OF EGRESS
--   Ang matag NUMERO (1 hangtod 27) kay expander na (tan-awa ExpanderEveryNumber sa FormPageViewModel.cs).
--   Version 1 niini kay usa ka dako nga "I. Means of egress" expander; gitangtang na kana (version 2):
--   ang mga numero ibalik sa seksyon, ug ang ulohan i-delete.
-- =====================================================================

UPDATE form_item SET parent_item_id = NULL
 WHERE parent_item_id = 'during-construction.egress';

DELETE FROM form_item WHERE id = 'during-construction.egress';
