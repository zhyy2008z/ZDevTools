using System;
using ZDevTools.Data;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace ZDevTools.Test.Data
{
    public class UnitTest_SqlHelper
    {
        SqlHelper h = GetSqlhelper();

        public static SqlHelper GetSqlhelper()
        {
            Console.WriteLine("创建Sql Helper");
            return new SqlHelper("server=(localdb)\\MSSqllocaldb;AttachDbFilename=|DataDirectory|\\Data\\datadb.mdf;MultipleActiveResultSets=true;Integrated Security=true;");
        }

        [Fact]
        public void New()
        {
            Assert.IsType<SqlHelper>(h);
        }

        [Fact]
        public void Execute1()
        {
            h.Execute("select count(*) from Books");
        }

        [Fact]
        public void Execute2()
        {
            var random = new Random();
            var pages = random.Next(1000);

            h.Execute($"update books set bookpages=@p1 where id= @p0", 1, pages);

            Assert.True(h.GetScalar<int>("select bookpages from books where id=@p0", 1) == pages);
        }

        [Fact]
        public void GetScalar1()
        {
            Assert.True(h.GetScalar<int>("select count(*) from Books") > 0);
        }

        [Fact]
        public void GetScalar2()
        {
            Assert.True(h.GetScalar<int>("select count(*) from books where {where}", h.CreateParameter("bookcategory", SqlDbType.Int, 3)) > 0);
        }

        [Fact]
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

            Assert.True(books.Count > 0);
        }

        [Fact]
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

            Assert.True(books.Count > 0);
        }

        [Fact]
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

            Assert.True(bookCategories.Count > 0);
        }


        [Fact]
        public void ExecuteReader4() //嵌套Reader，连接字符串需设置MultipleActiveResultSets=true
        {
            List<BookCategory> bookCategories = new List<BookCategory>();
            List<Book> books = new List<Data.Book>();

            h.Execute("select * from bookcategories", reader =>
            {
                while (reader.Read())
                {
                    var bookCategory = new BookCategory();
                    bookCategory.Id = reader.GetInt32(0);
                    bookCategory.CategoryName = reader.IsDBNull(1) ? null : reader.GetString(1);

                    h.Execute("select * from books where bookcategory=@id", reader2 =>
                    {
                        while (reader2.Read())
                        {
                            var book = new Book();
                            book.Id = reader2.GetInt32(0);
                            book.BookName = reader2.IsDBNull(1) ? null : reader2.GetString(1);
                            book.BookPages = reader2.IsDBNull(2) ? null : (int?)reader2.GetInt32(2);
                            book.BookCategory = reader2.IsDBNull(3) ? null : (int?)reader2.GetInt32(3);
                            books.Add(book);
                            bookCategory.Books.Add(book);
                        }
                    }, h.CreateParameter("@id", bookCategory.Id));

                    bookCategories.Add(bookCategory);
                }
            });

            foreach (var bookCategory in bookCategories)
            {
                Console.WriteLine("类别名称：" + bookCategory.CategoryName);
            }

            foreach (var book in books)
            {
                Console.WriteLine("书名：" + book.BookName);
            }

            Assert.True(bookCategories.Count > 0 && books.Count > 0);
        }

        [Fact]
        public void GetDataTable()
        {
            var datatable = h.GetDataTable("select * from books where bookcategory in ({in:0})", new int[] { 1, 3 });

            foreach (DataRow row in datatable.Rows)
            {
                Console.WriteLine("书名：" + row[1]);
            }

            Assert.True(datatable.Rows.Count > 0);
        }

        [Fact]
        public void GetDataset()
        {
            var dataset = h.GetDataSet("select * from books where {where}", h.CreateParameter("@id", 3));

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                Console.WriteLine("书名：" + row[1]);
            }

            Assert.True(dataset.Tables[0].Rows.Count > 0);
        }
    }
}
