using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterMelon_API.Models;
using WaterMelon_API.Services;

namespace WaterMelon_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {

        private readonly ItemService _itemService;
        private readonly EventService _eventService;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public ItemsController(ItemService itemService, EventService eventService, NotificationService notificationService, UserService userService)
        {
            _itemService = itemService;
            _eventService = eventService;
            _notificationService = notificationService;
            _userService = userService;
        }

        // GET: api/Items
        [HttpGet]
        [Authorize]
        public ActionResult<List<Item>> Get() => _itemService.GetAllItems();

        // GET: api/Items/5
        [HttpGet("{id}", Name = "GetItem")]
        public ActionResult<Item> GetFromId(string id)
        {
            var res = _itemService.GetFromItemId(id);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        // GET: api/Items/FromEvent/5
        [HttpGet]
        [Authorize]
        [Route("FromEvent/{id}")]
        public ActionResult<List<Item>> GetFromEvent(string id) => _itemService.GetItemsFromEvent(id);

        // POST: api/Items
        [HttpPost]
        [Authorize]
        public ActionResult<Item> Post([FromBody] ItemRequest itemRequest)
        {
            Item it = new Item(itemRequest);
            Item createdItem = _itemService.Create(it);
            if (createdItem == null)
            {
                return Unauthorized("L'objet existe déjà.");
            }

            Event tmp = _eventService.AddItemToList(createdItem.FromEvent, createdItem.Id);
            if (tmp == null)
            {
                return NotFound("L'événement n'a pas été trouvé.");
            }
            return CreatedAtRoute("Get", new { id = it.Id }, it);
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<Item> Put(string id, [FromBody] ItemRequest itemRequest)
        {
            var res = _itemService.GetFromItemId(id);
            if (res == null)
            {
                return NotFound();
            }
            return _itemService.UpdateItem(id, itemRequest);
        }

              //PUT: api/Items/Give/5
        [HttpPut]
        [Authorize]
        [Route("Give/{id}")]
        public ActionResult<Item> Give(string id, [FromBody] DonationRequest donationRequest)
        {
            var res = _itemService.GetFromItemId(id);
            if (res == null)
            {
                return NotFound();
            }
            
            Item item = _itemService.GiveItem(id, donationRequest.userId, donationRequest.quantity);
            if (item == null)
            {
                return NotFound();
            }

            Event contribEvent = _eventService.GetFromEventId(res.FromEvent);
            User user = _userService.Get(donationRequest.userId);
            if (contribEvent.Owner != user.Name)
            {
                NotificationRequest request = new NotificationRequest();
                request.Type = "Contribution";
                request.From = user.Name;
                request.To = contribEvent.Owner;
                request.EventId = res.FromEvent;

                Notification notif = new Notification(request);
                notif.About = user.Name + " a contribué à votre événement \"" + contribEvent.Name + "\"";
                _notificationService.Create(notif);
            }
            
            return item;
        }

        //PUT: api/Items/Pay/5
        [HttpPut]
        [Authorize]
        [Route("Pay/{id}")]
        public ActionResult<Item> Pay(string id, [FromBody] PayingRequest donationRequest)
        {
            var res = _itemService.GetFromItemId(id);
            if (res == null)
            {
                return NotFound();
            }

            Item item = _itemService.PayItem(id, donationRequest.userId, donationRequest.Amount);
            if (item == null)
            {
                return NotFound();
            }

            Event contribEvent = _eventService.GetFromEventId(res.FromEvent);
            User user = _userService.Get(donationRequest.userId);
            if (contribEvent.Owner != user.Name)
            {
                NotificationRequest request = new NotificationRequest();
                request.Type = "Contribution";
                request.From = user.Name;
                request.To = contribEvent.Owner;
                request.EventId = res.FromEvent;

                Notification notif = new Notification(request);
                notif.About = user.Name + " a contribué à votre événement \"" + contribEvent.Name + "\"";
                _notificationService.Create(notif);
            }

            return item;
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var res = _itemService.GetFromItemId(id);

            if (res == null)
            {
                return NotFound();
            }
            _eventService.RemoveItemFromList(res.FromEvent, id);
            _itemService.RemoveItemWithId(id);
            return StatusCode(200);
        }
    }
}
