﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IF.Lastfm.Core.Tests
{
    public static class TestHelper
    {
        private static readonly JsonSerializerSettings _testSerialiserSettings;

        static TestHelper()
        {
            _testSerialiserSettings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static string TestSerialise<T>(this T poco)
        {
            return JsonConvert.SerializeObject(poco, Formatting.Indented, _testSerialiserSettings);
        }

        public static string DifferencesTo<T>(this IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var first = String.Join(Environment.NewLine, expected);
            var second = String.Join(Environment.NewLine, actual);

            return first.DifferencesTo(second);
        }

        public static string DifferencesTo(this string first, string second)
        {
            const string lineDiffTemplate = "{0}A: {1}\n{0}B: {2}";
            var start = Environment.NewLine + Environment.NewLine + "Differences:" + Environment.NewLine;

            var sb = new StringBuilder(start);
            var sr1 = new StringReader(first);
            var sr2 = new StringReader(second);
            int count = 1;
            using (sr1)
            {
                using (sr2)
                {
                    while (true)
                    {
                        var line1 = sr1.ReadLine();
                        var line1Read = !String.IsNullOrEmpty(line1);
                        var line2 = sr2.ReadLine();
                        var line2Read = !String.IsNullOrEmpty(line2);

                        if (line1Read && line2Read)
                        {
                            if (line1 != line2)
                            {
                                var line = string.Format(lineDiffTemplate, count, line1, line2);
                                sb.AppendLine(line);
                            }
                        }
                        else if (!line1Read && !line2Read)
                        {
                            break;
                        }
                        else // one string still has lines
                        {
                            var line = string.Format(lineDiffTemplate, count, line1, line2);
                            sb.AppendLine(line);
                        }

                        count++;
                    }
                }
            }
            return sb.ToString();
        }

        public static bool LineRead(this StringReader reader, out string line)
        {
            line = reader.ReadLine();

            return !string.IsNullOrEmpty(line);
        }

        public static IEnumerable<T> WrapEnumerable<T>(this T t)
        {
            return new[] {t};
        }

        public static DateTime RoundToNearestSecond(this DateTime dt)
        {
            var ms = dt.Millisecond;

            return ms < 500
                ? dt.AddMilliseconds(-ms)
                : dt.AddMilliseconds(1000 - ms);
        }

        public static void AssertValues<T>(
            this PageResponse<T> pageResponse,
            bool success,
            int totalItems,
            int pageSize,
            int page,
            int totalPages) where T : new()
        {
            const string messageFormat = "Page response:\n{0}\n\nExpected {1} to equal {2}";
            var json = pageResponse.TestSerialise();
            Func<string, dynamic, string> testMessage = (property, count) => string.Format(messageFormat, json, property, count);

            Assert.IsTrue(pageResponse.Success == success, testMessage("success", success));
            Assert.IsTrue(pageResponse.TotalItems == totalItems, testMessage("totalitems", totalItems));
            Assert.IsTrue(pageResponse.PageSize == pageSize, testMessage("pagesize", pageSize));
            Assert.IsTrue(pageResponse.Page == page, testMessage("page", page));
            Assert.IsTrue(pageResponse.TotalPages == totalPages, testMessage("totalpages", totalPages));

            Assert.IsNotNull(pageResponse.Content, "page content is null");
            Assert.IsTrue(pageResponse.Content.Count == totalItems, testMessage("content length", totalItems));
        }
    }
}
