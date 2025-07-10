using Microsoft.AspNetCore.Mvc;
using TGramHunt.ViewModels.MainPage;

namespace TGramHunt.Pages.Shared.Components.AboutVoteButton
{
    public class AboutVoteButton : ViewComponent
    {
        public AboutVoteButton()
        {

        }

        public void OnGet()
        {
            //do nothing
        }

        public IViewComponentResult Invoke(ProductVoteViewModel productVoteViewModel)
        {
            return View(productVoteViewModel);
        }
    }
}