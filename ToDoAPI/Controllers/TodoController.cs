using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Models;
using System.Linq;

namespace ToDoAPI.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        private readonly ToDoContext _context;

        public ToDoController(ToDoContext context)
        {
            _context = context;

            if (_context.ToDoItems.Count() == 0)
            {
                _context.ToDoItems.Add(new ToDoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        //GET /api/toto
        [HttpGet] 
        public IEnumerable<ToDoItem> GetAll()
        {
            //returns 200 with JSON object of all items
            return _context.ToDoItems.ToList();
        }

        //GET /api/todo/{id}
        [HttpGet("{id}", Name = "GetToDo")] 
        public IActionResult GetById(long id)
        {
            var item = _context.ToDoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                //returns 404 response
                return NotFound();
            }
            //returns 200 response with JSON item
            return new ObjectResult(item);
        }


        //[FromBody] tells mvc to get the item from the body of http request
        [HttpPost]
        public IActionResult Create([FromBody] ToDoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.ToDoItems.Add(item);
            _context.SaveChanges();

            //returns 201 response (standard for HTTP POST sucess for new resource created at server
            //the location of newly created item is returned in header of the 201 response
            //uses the route with the "GetToDo" name to create the URL
            return CreatedAtRoute("GetToDo", new { id = item.Id }, item);
        }

        //put requires the client to send entire updated entity instead of changes
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] ToDoItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.ToDoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            _context.ToDoItems.Update(todo);
            _context.SaveChanges();
            //response is 204 no content for PUT
            return new NoContentResult();
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.ToDoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.ToDoItems.Remove(todo);
            _context.SaveChanges();
            //standard resoonse is 204 no content
            return new NoContentResult();
        }
    }
}
