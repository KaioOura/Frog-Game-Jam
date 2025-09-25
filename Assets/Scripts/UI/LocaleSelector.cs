using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
   private IEnumerator _setLocaleRoutine;
   
   public void ChangeLanguage(int localeId)
   {
      
      if (_setLocaleRoutine != null)
         StopCoroutine(_setLocaleRoutine);
      
      _setLocaleRoutine = SetLocale(localeId);
      StartCoroutine(_setLocaleRoutine);
   }
   
   private IEnumerator SetLocale(int localeID)
   {
      yield return LocalizationSettings.InitializationOperation;
      LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID]; 
   }
}
