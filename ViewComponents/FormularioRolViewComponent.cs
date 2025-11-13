using Microsoft.AspNetCore.Mvc;

namespace SIS_DIAF.ViewComponents
{
    public class FormularioRolViewComponent : ViewComponent
    {
        public FormularioRolViewComponent() { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
