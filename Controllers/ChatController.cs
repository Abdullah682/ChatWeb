using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

public class ChatController : Controller
{
    public IActionResult Index(string name, string room)
    {

        ViewBag.UserName = name;
        ViewBag.RoomCode = room;
        return View("Chat"); // Ye chat.cshtml ko load karega
    }

    public IActionResult JoinRoom(string userName, string roomName)
    {
        HttpContext.Session.SetString("userName", userName);
        HttpContext.Session.SetString("roomName", roomName);


        return View("ChatRoom");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // ✅ Session Data Remove
        return RedirectToAction("Index", "Home"); // ✅ Redirect Login Page
    }
   
}
