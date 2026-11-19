create table public.reviews (
  id          bigint generated always as identity primary key,
  author      text not null check (char_length(trim(author)) between 1 and 60),
  tour        text not null,
  rating      int  not null check (rating between 1 and 5),
  comment     text not null check (char_length(comment) between 10 and 500),
  deleted_at  timestamptz,                         -- soft delete
  created_at  timestamptz not null default now()
);

-- habit 1: the list page filters on deleted_at and sorts by newest
create index reviews_active_newest_idx
  on public.reviews (created_at desc)
  where deleted_at is null;

grant select, insert, update, delete on public.reviews to anon;   -- lock 1
alter table public.reviews enable row level security;             -- lock 2
create policy "read_all"   on public.reviews for select to anon using (true);
create policy "insert_all" on public.reviews for insert to anon with check (true);
create policy "update_all" on public.reviews for update to anon using (true) with check (true);
create policy "delete_all" on public.reviews for delete to anon using (true);