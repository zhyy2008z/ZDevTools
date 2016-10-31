using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZDevTools.Data;
using System.Collections.Generic;
using System.Data;

namespace ZDevTools.Test.Data
{
    [TestClass]
    public class UnitTest_SqlHelper
    {
        SqlHelper h = GetSqlhelper();

        public static SqlHelper GetSqlhelper()
        {
            Console.WriteLine("创建Sql Helper");
            return new SqlHelper("server=(localdb)\\MSSqllocaldb;AttachDbFilename=|DataDirectory|\\Data\\datadb.mdf;MultipleActiveResultSets=true;Integrated Security=true;");
        }


        [TestMethod]
        public void New()
        {
            Assert.IsInstanceOfType(h, typeof(SqlHelper));
        }

        [TestMethod]
        public void Execute1()
        {

            h.Execute("select count(*) from Books");
        }

        [TestMethod]
        public void Execute2()
        {
            var random = new Random();
            var pages = random.Next(1000);

            h.Execute($"update books set bookpages=@p1 where id= @p0", 1, pages);

            Assert.IsTrue(h.GetScalar<int>("select bookpages from books where id=@p0", 1) == pages);
        }

        [TestMethod]
        public void GetScalar1()
        {
            Assert.IsTrue(h.GetScalar<int>("select count(*) from Books") > 0);
        }

        [TestMethod]
        public void ExecuteReader1()
        {
            List<Book> books = new List<Data.Book>();

            h.Execute("select * from books", reader =>
            {
                while (reader.Read())
                {
                    var book = new Book();
                    book.Id = reader.GetInt32(0);
                    book.BookName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    book.BookPages = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                    book.BookCategory = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                    books.Add(book);
                }
            });

            foreach (var book in books)
            {
                Console.WriteLine("书名：" + book.BookName);
            }

            Assert.IsTrue(books.Count > 0);
        }

        [TestMethod]
        public void ExecuteReader2()
        {
            List<Book> books = new List<Data.Book>();

            h.Execute("select * from books where id in ({in:bookids})", reader =>
            {
                while (reader.Read())
                {
                    var book = new Book();
                    book.Id = reader.GetInt32(0);
                    book.BookName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    book.BookPages = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                    book.BookCategory = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
                    books.Add(book);
                }
            }, h.CreateInParameters(h.CreateInParameter("bookids", 1, 3, 5, 2)));

            foreach (var book in books)
            {
                Console.WriteLine("书名：" + book.BookName);
            }

            Assert.IsTrue(books.Count > 0);
        }

        [TestMethod]
        public void ExecuteReader3()
        {
            List<BookCategory> bookCategories = new List<BookCategory>();

            h.Execute("select * from bookcategories", reader =>
            {
                while (reader.Read())
                {
                    var bookCategory = new BookCategory();
                    bookCategory.Id = reader.GetInt32(0);
                    bookCategory.CategoryName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    bookCategories.Add(bookCategory);
                }
            });

            foreach (var bookCategory in bookCategories)
            {
                Console.WriteLine("类别名称：" + bookCategory.CategoryName);
            }

            Assert.IsTrue(bookCategories.Count > 0);
        }

        [TestMethod]
        public void GetDataTable()
        {
            var datatable = h.GetDataTable("select * from books where bookcategory in ({in:0})", new int[] { 1 ,3});

            foreach (DataRow row in datatable.Rows)
            {
                Console.WriteLine("书名：" + row[1]);
            }

            Assert.IsTrue(datatable.Rows.Count > 0);
        }

        [TestMethod]
        public void GetDataset()
        {
            var dataset = h.GetDataSet("select * from books where {where}", h.CreateParameter("@id", 3));

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                Console.WriteLine("书名：" + row[1]);
            }

            Assert.IsTrue(dataset.Tables[0].Rows.Count > 0);
        }

    }
}
