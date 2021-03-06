//
//  Author:
//       Timo Dörr <timo@latecrew.de>
//
//  Copyright (c) 2012 Timo Dörr
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;
using NUnit.Framework;
using System.Xml;
using Tomboy.Sync;
using System.Text;
using System.IO;
using Tomboy.Tags;
using System.Linq;

namespace Tomboy.Sync.Web
{
	[TestFixture()]
	public class SyncObjectsTest
	{
		SyncManifest sampleManifest;

		[SetUp]
		public void Setup ()
		{
			var manifest = new SyncManifest ();
			manifest.LastSyncDate = DateTime.UtcNow - new TimeSpan (1, 0, 0);
			manifest.LastSyncRevision = 2;

			manifest.NoteRevisions.Add ("1234-5678-9012-3456", 4);
			manifest.NoteRevisions.Add ("1111-2222-3333-4444", 9293);
			manifest.NoteRevisions.Add ("6666-2222-3333-4444", 17);
			
			manifest.NoteDeletions.Add ("1111-11111-1111-1111", "Deleted note 1");
			manifest.NoteDeletions.Add ("1111-11111-2222-2222", "Gelöschte Notiz 2");

			sampleManifest = manifest;
		}
		[Test()]
		public void ReadWriteSyncManifest ()
		{
			// write the sample manifest to XML
			StringBuilder builder = new StringBuilder ();
			XmlWriter writer = XmlWriter.Create (builder);

			SyncManifest.Write (writer, sampleManifest);

			// read in the results
			var textreader = new StringReader (builder.ToString ());
			var xmlreader = new XmlTextReader (textreader);
			var manifest = SyncManifest.Read (xmlreader);

			// verify
			Assert.AreEqual (sampleManifest.LastSyncDate, manifest.LastSyncDate);
			Assert.AreEqual (sampleManifest.LastSyncRevision, manifest.LastSyncRevision);

			Assert.AreEqual (sampleManifest.NoteRevisions.Count, manifest.NoteRevisions.Count);

			foreach (var kvp in sampleManifest.NoteRevisions) {
				Assert.That (manifest.NoteRevisions.ContainsKey (kvp.Key));
				Assert.That (manifest.NoteRevisions [kvp.Key] == kvp.Value);
			}
			foreach (var kvp in sampleManifest.NoteDeletions) {
				Assert.That (manifest.NoteDeletions.ContainsKey (kvp.Key));
				Assert.That (manifest.NoteDeletions[kvp.Key] == kvp.Value);
			}

		}
		[Test]
		public void ReadWriteEmptySyncManifest ()
		{
			var empty_manifest = new SyncManifest ();

			// write the sample manifest to XML
			StringBuilder builder = new StringBuilder ();
			XmlWriter writer = XmlWriter.Create (builder);

			SyncManifest.Write (writer, empty_manifest);

			// read in the results
			var textreader = new StringReader (builder.ToString ());
			var xmlreader = new XmlTextReader (textreader);
			var manifest = SyncManifest.Read (xmlreader);
		}

	}
}
